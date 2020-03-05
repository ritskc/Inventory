import { Injectable } from '@angular/core';
import { DataColumn } from '../../models/dataColumn.model';

@Injectable({
  providedIn: 'root'
})
export class JsonToCsvExporterService {

  constructor() { }

  export(filename: string, fileformat: string, data: any[], columns: any[]) {

    var dataToWrite: string = '';
    var headers = Object.keys(data[0]);

    columns.forEach(column => {
      if (column.actions.length == 0)
        dataToWrite += column.headerText + ",";
    });
    dataToWrite += String.fromCharCode(13);

    data.forEach(item => {
      columns.forEach(column => {
        if (typeof(item[column.value]) !== "object" && column.actions.length == 0) {
          var valueToPrint = item[column.value];
          if (typeof(valueToPrint) === 'string') valueToPrint = valueToPrint.replace(/\,/gi, "");
          dataToWrite += valueToPrint + ","
        }
      });
      dataToWrite += String.fromCharCode(13);
    });

    var blob = new Blob([dataToWrite], {type: 'text/plain'});
    var mouseEvent = document.createEvent('MouseEvents'),
    anchor = document.createElement('a');
    anchor.download = `${filename}.${fileformat}`;
    anchor.href = window.URL.createObjectURL(blob);
    anchor.dataset.downloadurl = ['text/json', anchor.download, anchor.href].join(':');
    mouseEvent.initEvent('click', true, false);
    anchor.dispatchEvent(mouseEvent);
  }
}
