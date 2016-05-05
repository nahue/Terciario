import AsistenciasList from './AsistenciasList';
import React from 'react';
import {RefreshIndicator, Snackbar} from 'material-ui';
import axios from 'axios';

export default class AsistenciasContainer extends React.Component {
  constructor() {
    super();
    this.state = {
      alumnos: [],
      isSaving: false,
      isLoading: false,
      openNotification: false,
      message: '',
      bodyStyle: null
    }
  }
  componentDidMount() {
    this.setState({isLoading: true});
    $.ajax({
      url: `/api/Cursos/GetAlumnos/${this.props.params.course_id}`,
      dataType: 'json',
      success: function(alumnos) {
        this.setState({
          alumnos: alumnos,
          isLoading: false
        });
      }.bind(this)
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
  exit() {
    this.props.history.replaceState(null, '/')
  }

  _save(data) {
    this.setState({isSaving: true});
    axios.post(`/api/Cursos/GuardarAsistencias/${this.props.params.course_id}`, data)
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
    console.log("RESET");
    this.setState({
      openNotification: false,
      message: '',
      bodyStyle: null
    })
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
