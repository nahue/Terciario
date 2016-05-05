module.exports = {
  path: '/Curso/:course_id/Asistencias/Nueva',

  getComponent(location, cb) {
    cb(null, require('../components/Asistencias/container'));
  }
};
