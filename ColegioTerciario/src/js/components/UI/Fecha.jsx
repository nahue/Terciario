import React from 'react';
import { DatePicker } from 'material-ui';
import XDate from 'xdate';
const IntlPolyfill = require('intl');
require('intl/locale-data/jsonp/es-AR');

export default class FechaComponent extends React.Component {
  _formatDate(dateString) {
    let date = new XDate(dateString);
    return date.toString("dd/MM/yyyy");
  }

  render() {

    let DateTimeFormat = IntlPolyfill.DateTimeFormat;

    return (
        <DatePicker
          formatDate={this._formatDate.bind(this)}
          DateTimeFormat={DateTimeFormat}
          locale="es-AR"
          wordings={{ok: 'OK', cancel: 'Cancelar'}}
          {...this.props}
        />
      )
    }
  };
