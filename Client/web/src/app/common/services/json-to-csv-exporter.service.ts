import { Injectable } from '@angular/core';
import { DataColumn } from '../../models/dataColumn.model';

@Injectable({
  providedIn: 'root'
})
export class JsonToCsvExporterService {

  constructor() { }

  export(filename: string, fileformat: string, data: any[]) {

    var dataToWrite: string = '';
    var headers = Object.keys(data[0]);

    headers.forEach(item => {
      dataToWrite += item + ",";
    });
    dataToWrite += String.fromCharCode(13);

    data.forEach(item => {
      headers.forEach(header => {
        dataToWrite += item[header] + ","
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
