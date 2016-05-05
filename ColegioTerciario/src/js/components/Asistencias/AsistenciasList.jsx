import React from 'react';
import axios from 'axios';
import {Toggle, RaisedButton, Snackbar } from 'material-ui';
import FechaComponent from '../UI/Fecha';

class Lista extends React.Component {
  render() {
    return (
      <div className="table-responsive">
        <table className="table table-bordered">
          <tbody>
            {this.props.alumnos.map((alumno) => {
              return (
                <tr key={alumno.ID}>
                  <td style={{width: '140px'}}>
                    <Toggle
                      name="asistencia[]"
                      label="Presente"
                      value={alumno.ID}
                    />
                  </td>
                  <td>{alumno.APELLIDO}, {alumno.NOMBRE}</td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
    )
  }
}

export default class Asistencias extends React.Component {
  constructor() {
    super();

  }

  _toggle(id, event, value) {
    let index = this.props.alumnos.findIndex((alumno) => {return alumno.ID == id})
  }

  _handleSubmit(e) {
    e.preventDefault();
    var form = $(this.refs.form);

    let data = form.serialize();
    this.props.save(data);
    /*
    axios.post(`/api/Cursos/GuardarAsistencias/${this.props.course_id}`, data)
      .then((response) => {
        this.setState({
          openNotification: true
        });
      })
      .catch((response) => {
        this.setState({
          message: response.data.Message,
          openNotification: true,
          bodyStyle: {backgroundColor: "#f44336"}
        });
      });
    */
  }

  render() {
    return (
      <form ref="form" onSubmit={this._handleSubmit.bind(this)}>
      <div className="block-header">
        <h2>Cargar Asistencias</h2>
        <div className="actions">
          {this.props.info_curso ?
            <div>{this.props.info_curso.Carrera} <br />
            {this.props.info_curso.Ciclo} | {this.props.info_curso.Nombre} | {this.props.info_curso.Materia}
            </div>
            : null
          }
        </div>
      </div>

      <div className="card">
        <div className="card-body card-padding">
          <input type="submit" value="Guardar" disabled={this.props.isLoading || this.props.isSaving}/>

          <FechaComponent
            hintText="Seleccionar Fecha"
            mode="landscape"
            name="FECHA"
            defaultDate={new Date()}/>
        </div>
      </div>
      <div className="card">

          {this.props.isLoading == true ?
            null
            :
            this.props.alumnos.length > 0 ?
              <div className="card-body">
                <Lista alumnos={this.props.alumnos} toggle={this._toggle.bind(this)}/>
              </div>
              :
              <div className="card-body card-padding">
                No se encontraron alumnos
              </div>

          }
      </div>





      </form>

    )
  }
}
