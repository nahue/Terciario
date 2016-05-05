import React from 'react';
import $ from 'jquery';
import GriddleWithCallback from '../lib/GriddleWithCallback';
import GriddleActionsComponent from '../lib/GriddleActionsComponent';
import UserStore from '../../stores/userStore';
import Reflux from 'reflux';
import AuthorizationMixin from '../../core/AuthorizationMixin';
import reactMixin from 'react-mixin';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import * as AreaDocentesActions from '../../actions/areaDocentesActions';


class AreaDocentesMain extends React.Component {

  shouldComponentUpdate(nextProps, nextState) {
    if (this.props.areaDocentes.refreshCourseList == false) {
      return false
    }
    return true;
  }
  _getJsonData(filterString, sortColumn, sortAscending, page, pageSize, callback) {
    $.get(`/api/Cursos/ObtenerCursos?docenteId=${this.state.user.data.Persona}`,
      {
        Pagina: page,
        RegistrosPorPagina: pageSize,
        OrdenarPorColumna: sortColumn,
        OrdenarAsc: sortAscending
      },
      function (data) {
        callback({
          totalResults: data.CantidadResultados,
          results: data.Resultados,
          pageSize
        });
      });
  }

  _rowClick(rowData, event) {
    this.props.actions.changeSelectedCourse({
      id: rowData.props.data.ID,
      amountExams: rowData.props.data.CANTIDAD_PARCIALES
    });
  }

  render() {
    const columns = [
      'CICLO_ANIO',
      'CARRERA_NOMBRE',
      'MATERIA_NOMBRE',
      'MATERIA_X_CURSO_CURSO_NOMBRE',
      'SEDE_NOMBRE',
      'ID'
    ];
    const columnMeta = [
      {
        columnName: 'CICLO_ANIO',
        displayName: 'Año'
      }, {
        columnName: 'CARRERA_NOMBRE',
        displayName: 'Carrera'
      }, {
        columnName: 'MATERIA_NOMBRE',
        displayName: 'Materia'
      }, {
        columnName: 'MATERIA_X_CURSO_CURSO_NOMBRE',
        displayName: 'Curso'
      }, {
        columnName: 'SEDE_NOMBRE',
        displayName: 'Sede'
      }, {
        columnName: 'ID',
        "visible": false
      }
    ];

    return (
      <div className="card light">
        <div className="card-header">
          <h2>Sus Cursos</h2>
        </div>
        <div className="card-body">
          <GriddleWithCallback ref="w"
                  getExternalResults={this._getJsonData.bind(this)}
                  columnMetadata={columnMeta}
                  columns={columns}
                  loadingText="Cargando..."
                  tableClassName="table table-vmiddle"
                  noDataMessage="No se encontraron resultados"
                  nextText="Siguiente"
                  onRowClick={this._rowClick.bind(this)}/>
        </div>
      </div>
    );
  }
}

reactMixin.onClass(AreaDocentesMain, Reflux.connect(UserStore));
reactMixin.onClass(AreaDocentesMain, AuthorizationMixin);


const mapStateToProps = (store) => {
  return {
    areaDocentes: store.areaDocentes
  };
};

function mapDispatchToProps(dispatch) {
  return {
    actions: bindActionCreators(AreaDocentesActions, dispatch)
  };
}
export default connect(mapStateToProps, mapDispatchToProps)(AreaDocentesMain);
