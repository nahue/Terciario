import React from 'react';
import DashboardDocentes from './DashboardDocentes';
import DashboardBedeles from './DashboardBedeles';
import AreaDocentes from '../AreaDocentes'

class InicioComponent extends React.Component {
  esDocente() {
    return User.isInRole('Docente');
  }

  render() {
    let contenido;

    if (this.esDocente()) {
      contenido = <AreaDocentes />;
    } else {
      contenido = <DashboardBedeles />;
    }
    return contenido;
  }
}

export default InicioComponent;
