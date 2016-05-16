import AsistenciasList from './AsistenciasList';
import React from 'react';
import {RefreshIndicator, Snackbar} from 'material-ui';
import axios from 'axios';
import moment from 'moment';

export default class AsistenciasContainer extends React.Component {
  constructor() {
    super();
    this.state = {
      alumnos: [],
      isSaving: false,
      isLoading: false,
      openNotification: false,
      message: '',
      bodyStyle: null,
      fecha: moment().format("DD/MM/YYYY")
    }
  }

  _loadData(fecha) {
    $.ajax({
      url: `/api/Cursos/GetAlumnosParaInscripciones/${this.props.params.course_id}?FECHA=${fecha}`,
      dataType: 'json',
      success: (alumnos) => {
        this.setState({
          alumnos: alumnos,
          isLoading: false
        });
      }
    });

    $.ajax({
      url: `/api/Cursos/Info/${this.props.params.course_id}?instancia=P1`,
      dataType: 'json',
      success: (info) => {
        this.setState({
          info: info
        });
      }
    })
  }

  componentDidMount() {
    this.setState({isLoading: true});
    this._loadData(this.state.fecha);
  }

  exit() {
    this.props.history.replaceState(null, '/')
  }

  _save(date) {
    this.setState({isSaving: true});

    axios.post(`/api/Cursos/GuardarAsistencias/${this.props.params.course_id}?FECHA=${this.state.fecha}`, this.state.alumnos)
      .then((response) => {
        this.setState({
          message: 'Asistencias guardadas correctamente',
          openNotification: true,
          isSaving: false
        });
        this.exit();
      })
      .catch((response) => {
        this.setState({
          message: response.data.Message,
          openNotification: true,
          isSaving: false,
          bodyStyle: {backgroundColor: "#f44336"}
        });
      });

  }

  resetNotification() {
    this.setState({
      openNotification: false,
      message: '',
      bodyStyle: null
    })
  }

  _onChangeDate(e, newDate){
    this.setState({fecha: moment(newDate).format("DD/MM/YYYY")});
    this._loadData(moment(newDate).format("DD/MM/YYYY"));
  }

  _onToggle(dni, event, value) {
    console.log(dni, event, value);
    let alumnos = this.state.alumnos.map((alumno) => {
      if (alumno.DNI == dni) {
        alumno.PRESENTE = value;
      }
      return alumno;
    });

    this.setState({alumnos});

  }

  render() {
    let style = {
      container: {
        position: 'fixed',
        bottom: '30px',
        right: '100px',
        display: this.state.isLoading || this.state.isSaving ? "block" : "none"
      },
      refresh: {
        position: 'relative'
      }
    }
    return (
      <div>
        <AsistenciasList
          course_id={this.props.params.course_id}
          info_curso={this.state.info}
          alumnos={this.state.alumnos}
          exit={this.exit.bind(this)}
          isLoading={this.state.isLoading}
          isSaving={this.state.isSaving}
          save={this._save.bind(this)}
          fecha={this.state.fecha}
          onChangeDate={this._onChangeDate.bind(this)}
          onToggle={this._onToggle.bind(this)}
        />;
        <div style={style.container}>
          <RefreshIndicator
            size={50}
            left={70}
            top={0}
            loadingColor={"#FF9800"}
            status='loading'
            style={style.refresh}
          />
        </div>
        <Snackbar
          open={this.state.openNotification}
          message={this.state.message}
          autoHideDuration={4000}
          onRequestClose={this.resetNotification.bind(this)}
          bodyStyle={this.state.bodyStyle}
        />
      </div>
    )
  }
}
