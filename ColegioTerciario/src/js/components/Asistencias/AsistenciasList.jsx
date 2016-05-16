import React from 'react';
import axios from 'axios';
import {Toggle, RaisedButton, Snackbar } from 'material-ui';
import FechaComponent from '../UI/Fecha';
import ReactDOM from 'react-dom';

class Lista extends React.Component {
  render() {
    return (
      <div className="table-responsive">
        <table className="table table-bordered">
          <tbody>
            {this.props.alumnos.map((alumno) => {
              return (
                <tr key={alumno.DNI}>
                  <td style={{width: '140px'}}>
                    <Toggle
                      name="asistencia[]"
                      label="Presente"
                      value={alumno.ID}
                      defaultToggled={this.props.alumnos[this.props.alumnos.indexOf(alumno)].PRESENTE}
                      onToggle={this.props.onToggle.bind(this, alumno.DNI)}
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

  _onChangeDate() {
    this.props.onChangeDate();
  }

  shouldComponentUpdate(nextProps, nextState) {
    console.log(nextProps, nextState);
    if (nextProps.alumnos.length == 0) {
      return false;
    } else {
      if (this.props.alumnos.length == nextProps.alumnos.length) {
        var oldAlumnos = [];
        var newAlumnos = [];

        this.props.alumnos.map((alumno) => {
           if (alumno.PRESENTE == true) {
               oldAlumnos.push(alumno);
           }
        });

        nextProps.alumnos.map((alumno) => {
           if (alumno.PRESENTE == true) {
               newAlumnos.push(alumno);
           }
        });

        if (oldAlumnos.length != newAlumnos.length) {
          return true;
        }


        return false;
      }
      return true;
    }
  }

  _handleSubmit(e) {
    e.preventDefault();
    var form = $(this.refs.form);

    let data = form.serialize();
    this.props.save(data);
  }

  render() {
    console.log("RENDER LIST");
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
            ref="FECHA"
            defaultValue={this.props.fecha}
            onChange={this.props.onChangeDate}/>
        </div>
      </div>
      <div className="card">

          {this.props.isLoading == true ?
            null
            :
            this.props.alumnos.length > 0 ?
              <div className="card-body">
                <Lista alumnos={this.props.alumnos} onToggle={this.props.onToggle}/>
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
