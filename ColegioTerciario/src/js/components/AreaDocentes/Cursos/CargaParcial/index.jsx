
export default {
  path: '/area-docentes/cursos/:idCurso/cargaParcial/:parcial',

  getComponent(location, cb) {
    // require.ensure([], (require) => {
    //  cb(null, require('./main'));
    // });

    cb(null, require('./CargaParcial'));
  }
};
