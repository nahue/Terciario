import React from 'react';
import $ from 'jquery';
import {Link} from 'react-router';
import GriddleWithCallback from '../../lib/GriddleWithCallback';

const FechaComponent = React.createClass({
  propTypes: {
    data: React.PropTypes.string
  },

  formatDate(date) {
    let d = date.getDate();
    let m = date.getMonth() + 1;
    const y = date.getFullYear();

    if (d.toString().length === 1) { d = '0' + d; }
    if (m.toString().length === 1) { m = '0' + m; }
    return d + '/' + m + '/' + y;
  },

  render() {
    const date = new Date(this.props.data);

    return <div>{this.formatDate(date)}</div>;
  }
});

const ActionsComponent = React.createClass({
  propTypes: {
    data: React.PropTypes.number
  },

  render() {
    return (
      <div className="dropdown">
        <a href="#" className="dropdown-toggle btn btn-link btn-icon waves-effect"
           data-toggle="dropdown" aria-expanded="true">
          <i className="zmdi zmdi-more-vert"></i>
        </a>

        <ul className="dropdown-menu pull-right bgm-bluegray">
          <li>
            <Link to={`/equivalencias/${this.props.data}/editar`}>
              Editar
            </Link>
          </li>
        </ul>
      </div>
    );
  }
});

const EquivalenciasMain = React.createClass({

  onSelectAll(isSelected) {
    // console.log("is select all: " + isSelected);
  },

  onRowSelect(row, isSelected) {
    // console.log("selected: " + isSelected)
  },

  _getJsonData(filterString, sortColumn, sortAscending, page, pageSize, callback) {
    $.ajax({
      url: '/api/Equivalencias/GetEquivalencias',
      dataType: 'json',
      data: {Pagina: page, RegistrosPorPagina: pageSize},
      success: (data) => {
        callback({
          results: data.Resultados,
          totalResults: data.CantidadResultados,
          pageSize
        });
      },
      error: (xhr, status, err) => {
        // TODO: mostrar notificacion de errores
        // console.error(this.props.url, status, err.toString());
      }
    });
  },

  columnaAcciones(cell, row) {
    return <Link to={`/equivalencias/${cell}/editar`}>Editar</Link>;
  },

  render() {
    const columns = [
      'EQUIVALENCIA_FECHA',
      'EQUIVALENCIA_NRO_DISPOSICION',
      'EQUIVALENCIA_ALUMNO_NOMBRE',
      'EQUIVALENCIA_CARRERA_NOMBRE',
      'ID'
    ];
    const columnMeta = [
      {
        columnName: 'ID',
        displayName: '',
        customComponent: ActionsComponent
      },
      {
        columnName: 'EQUIVALENCIA_FECHA',
        displayName: 'Fecha',
        customComponent: FechaComponent
      }, {
        columnName: 'EQUIVALENCIA_NRO_DISPOSICION',
        displayName: 'Nro de Disposicion'
      }, {
        columnName: 'EQUIVALENCIA_ALUMNO_NOMBRE',
        displayName: 'Alumno'
      }, {
        columnName: 'EQUIVALENCIA_CARRERA_NOMBRE',
        displayName: 'Carrera'
      }
    ];

    return (
      <div>
        <div className="block-header">

        </div>
        <div className="card">
          <div className="card-header ch-alt m-b-20">
            <h2>
              Equivalencias
            </h2>
            <Link to="/equivalencias/agrega"
              className="btn bgm-cyan btn-float waves-effect waves-circle waves-float">
              <i className="zmdi zmdi-plus"></i>
            </Link>
          </div>

          <GriddleWithCallback ref="w"
              getExternalResults={this._getJsonData}
              columnMetadata={columnMeta}
              resultsPerPage={10}
              columns={columns}
              loadingText="Cargando..."
              noDataMessage="No se encontraron resultados"
              tableClassName="table table-vmiddle"
            />
        </div>
      </div>
    );
  }
});

export default EquivalenciasMain;

