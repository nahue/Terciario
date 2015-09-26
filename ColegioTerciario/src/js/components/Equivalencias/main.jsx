import React from 'react'
import {ReactBsTable, BootstrapTable, TableHeaderColumn }  from "react-bootstrap-table"
import Component from '../Component/main'
import {Link} from 'react-router'
import mui, {MenuItem,MenuDivider,IconMenu, IconButton, MoreVertIcon} from 'material-ui'
import GriddleWithCallback from '../lib/GriddleWithCallback'

require('react-bootstrap-table/css/react-bootstrap-table.min.css')

class FechaComponent extends Component {
  constructor(props) {
    super(props);
  }
  render() {
    let date = new Date(this.props.data)
    return <div>{this.formatDate(date)}</div>;
  }
}

class ActionsComponent extends React.Component {
  render() {
    return <div><Link to={`/equivalencias/${this.props.data}/editar`}>Editar</Link></div>;
  }
}

export default class EquivalenciasMain extends Component {

  onSelectAll(isSelected) {
    console.log("is select all: " + isSelected);
  }

  onRowSelect(row, isSelected) {
    console.log("selected: " + isSelected)
  }

  constructor(props) {
    super(props);
    this.state = {data: []}
    this.selectRowProp = {
      mode: "checkbox",
      clickToSelect: true,
      bgColor: "rgb(238, 193, 213)",
      onSelect: this.onRowSelect,
      onSelectAll: this.onSelectAll
    };
  }

  _getJsonData(filterString, sortColumn, sortAscending, page, pageSize, callback) {

    $.ajax({
      url: '/api/Equivalencias/GetEquivalencias',
      dataType: 'json',
      data: { Pagina: page, RegistrosPorPagina: pageSize},
      success: function(data) {
        callback({
          results: data.Resultados,
          totalResults: data.CantidadResultados,
          pageSize: pageSize
        });

      }.bind(this),
      error: function(xhr, status, err) {
        console.error(this.props.url, status, err.toString());
      }.bind(this)
    })
  }

  columnaAcciones(cell, row) {
    return <Link to={'/equivalencias/'+cell+'/editar'}>Editar</Link>;
  }

  render() {
    let columnMeta = [
      {
        "columnName": "ID",
        "customComponent": ActionsComponent
      },
      {
        "columnName": "EQUIVALENCIA_FECHA",
        "displayName": "Fecha",
        "customComponent": FechaComponent
      }, {
        "columnName": "EQUIVALENCIA_NRO_DISPOSICION",
        "displayName": "Nro de Disposicion"
      }, {
        "columnName": "EQUIVALENCIA_ALUMNO_NOMBRE",
        "displayName": "Alumno"
      }, {
        "columnName": "EQUIVALENCIA_CARRERA_NOMBRE",
        "displayName": "Carrera"
      }
    ];
    return (
        <div className="portlet light">
            <Link to="agrega-equivalencias">Agrega</Link>

            <GriddleWithCallback ref="w"
                getExternalResults={this._getJsonData.bind(this)}
                columnMetadata = {columnMeta}
                resultsPerPage={10}
                columns={["ID","EQUIVALENCIA_FECHA", "EQUIVALENCIA_NRO_DISPOSICION", "EQUIVALENCIA_ALUMNO_NOMBRE", "EQUIVALENCIA_CARRERA_NOMBRE"]}
                loadingText = "Cargando..."
                noDataMessage = "No se encontraron resultados"/>
        </div>
    );
  }
}
