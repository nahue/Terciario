import React from 'react';
// import Wizard from 'react-wizard';
import Paso1Component from './Pasos/1';
import Paso2Component from './Pasos/2';
import Paso3Component from './Pasos/3';
import Paso4Component from './Pasos/4';
import Paso5Component from './Pasos/5';
import Paso6Component from './Pasos/6';

import {Tab, Tabs} from 'react-bootstrap';
import Notification from 'Notification';
import _ from 'lodash';
import axios from 'axios';
import {RaisedButton, Dialog} from 'material-ui';

export default React.createClass({
  getInitialState() {
    return {
      key: 1,
      showModal: false
    };
  },

  _checkDNI(dni) {
    return axios.get('/api/Inscripciones/VerificarHabilitacion', {
      params: {
        dni
      }
    });
  },

  _handleSelect(key) {
    let valid = false;

    if (key === 1) {
      valid = true;
    } else if (key === 2) {
      const state = this.refs['paso' + (key - 1)].state;

      if (!state.INSCRIPCIONES_DOCUMENTO_NUMERO) {
        Notification.error('Por favor ingrese su documento');
      } else if (!/[0-9]{7,}/.test(state.INSCRIPCIONES_DOCUMENTO_NUMERO)) {
        Notification.error('Paso 1: DNI Invalido');
      } else {
        Notification.showLoading('Verificando DNI...');
        this._checkDNI(state.INSCRIPCIONES_DOCUMENTO_NUMERO).then((result) => {
          if (!state.INSCRIPCIONES_NOMBRE ||
            !state.INSCRIPCIONES_APELLIDO ||
            !state.INSCRIPCIONES_DOCUMENTO_NUMERO) {
            Notification.error('Paso 1: Todos los campos son obligatorios');
          } else {
            Notification.clearNotifications();
            this.setState({key});
            return;
          }
        }).catch((result) => {
          Notification.error(
            'Paso 1: Ya se inscribio, ante cualquier problema acerquese a la sede'
          );
        });
      }
    } else if (key === 3) {
      const state = this.refs['paso' + (key - 1)].state;

      if (!state.INSCRIPCIONES_EMAIL || !state.INSCRIPCIONES_EMAIL2) {
        Notification.error('Paso 2: Todos los campos son obligatorios');
      } else if (state.INSCRIPCIONES_EMAIL !== state.INSCRIPCIONES_EMAIL2) {
        Notification.error('Paso 2: Los emails son distintos');
      } else if (
        !/^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/.test(state.INSCRIPCIONES_EMAIL)
      ) {
        Notification.error('Paso 2: Debe ingresar un email valido');
      } else {
        valid = true;
      }
    } else if (key === 4) {
      const state = this.refs['paso' + (key - 1)].state;

      if (!state.INSCRIPCIONES_NACIMIENTO_FECHA ||
        !state.INSCRIPCIONES_NACIMIENTO_PAIS_ID ||
        !state.INSCRIPCIONES_NACIMIENTO_PROVINCIA_ID ||
        !state.INSCRIPCIONES_NACIMIENTO_CIUDAD_ID) {
        Notification.error('Paso 3: Todos los campos son obligatorios');
      } else {
        valid = true;
      }
    } else if (key === 5) {
      const state = this.refs['paso' + (key - 1)].state;

      if (!state.INSCRIPCIONES_SEXO ||
        !state.INSCRIPCIONES_DOMICILIO ||
        !state.INSCRIPCIONES_NACIMIENTO_BARRIO_ID ||
        !state.INSCRIPCIONES_TELEFONO) {
        Notification.error('Paso 4: Todos los campos son obligatorios');
      } else {
        valid = true;
      }
    } else if (key === 6) {
      const state = this.refs['paso' + (key - 1)].state;

      if (!state.INSCRIPCIONES_CARRERA_ID) {
        Notification.error('Paso 5: Debe introducir la carrera');
      } else {
        valid = true;
      }
    } else {
      valid = true;
    }

    if (valid) {
      this.setState({key});
    }
  },

  _onSave() {
    const datos = _.merge(
      this.refs.paso1.state,
      this.refs.paso2.state,
      this.refs.paso3.state,
      this.refs.paso4.state,
      this.refs.paso5.state
    );

    Notification.showLoading('Guardando Inscripcion...');
    axios.post('/api/Inscripciones/PostInscripciones', datos)
    .then(function (response) {
      Notification.success('Su inscripcion fue guardada.');
      const id = response.data.ID;

      location.href = '/Publico/Inscripciones/ImprimirInscripcion/' + id;
    })
    .catch(function (response) {
      Notification.error(
        'Ocurrio un error, si el problema persiste comuniquese con la institucion.'
      );
    });
  },

  _onAskConfirmation() {
    this.setState({showModal: true});
  },

  _onPrev() {
    const prevKey = parseInt(this.state.key, 10) - 1;

    this._handleSelect(prevKey);
  },

  _onNext() {
    const nextKey = parseInt(this.state.key, 10) + 1;

    this._handleSelect(nextKey);
  },

  render() {
    // Standard Actions
    const standardActions = [
      {text: 'Imprimir Pre-Inscripcion', onTouchTap: this._onSave, ref: 'submit'}
    ];

    return (
      <div className="row">
        <div className="col-md-8 col-md-offset-2">
          <div className="card z-depth-4-bottom">
            <Dialog
              ref="modal"
              title="Importante"
              actions={standardActions}
              actionFocus="submit"
              open={this.state.showModal}
              modal={true}>
              <p>
                Esta a punto de finalizar la PRE-Inscripción, recuerde que para hacer efectiva la
                inscripciones usted deberá presentar en la institución el "Formulario" que se
                generara en este proceso junto a la documentación adicional solicitada.
              </p>
              <p>
                Documentación adicional: <b>A)</b> Fotocopia autenticada del título secundario.
                <b>B)</b> Fotocopia del documento de identidad.
                <b>C)</b> 2 (dos) foto carnet. De acogerse el postulante a las
                prescripciones del Art. 7° del a Ley Nacional 24521 por no haber cumplimentado sus
                estudios de nivel medio, deberá presentar sin excepción las constancias
                pertinentes que acrediten los estudios cursados, las correspondientes que
                acrediten la experiencia y/o idoneidad que se corresponda con la carrera
                a la cual se inscribe, y rendir oportunamente la
                evaluación establecida en la normativa de mención.
              </p>
              <p>
                Una copia del archivo PDF de PRE-Inscripcion será enviada a su correo electrónico
              </p>
            </Dialog>

            <div className="card-header">
              <h2>
                Formulario de Inscripcion
              </h2>
            </div>
            <div className="card-body card-padding">
              <div className="row">
                <Tabs
                  className="inscripcionesTabs"
                  activeKey={this.state.key}
                  onSelect={this._handleSelect}>
                  <Tab eventKey={1} title="Paso 1" disabled={true}>
                    <Paso1Component ref="paso1" />
                  </Tab>
                  <Tab eventKey={2} title="Paso 2" disabled={true}>
                    <Paso2Component ref="paso2" />
                  </Tab>
                  <Tab eventKey={3} title="Paso 3" disabled={true}>
                    <Paso3Component ref="paso3" />
                  </Tab>
                  <Tab eventKey={4} title="Paso 4" disabled={true}>
                    <Paso4Component ref="paso4" />
                  </Tab>
                  <Tab eventKey={5} title="Paso 5" disabled={true}>
                    <Paso5Component ref="paso5" />
                  </Tab>
                  <Tab eventKey={6} title="Paso 6" disabled={true}>
                    <Paso6Component
                      ref="paso6"
                      onSave={this._onAskConfirmation}/>
                  </Tab>
                </Tabs>

                <div className="pull-left" style={{marginLeft: '10px'}}>
                  <RaisedButton
                    onClick={this._onPrev}
                    label="Anterior"
                    secondary={true}
                    disabled={this.state.key === 1}
                  />
                </div>
                {this.state.key !== 6 ?
                  <div className="pull-right" style={{marginRight: '10px'}}>
                    <RaisedButton
                      onClick={this._onNext}
                      label="Siguiente"
                      secondary={true}
                    />
                  </div> : null}

              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
});
