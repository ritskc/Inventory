import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';

@Component({
  selector: 'simple-grid',
  templateUrl: './simple-grid.component.html',
  styleUrls: ['./simple-grid.component.scss']
})
export class SimpleGridComponent implements OnInit {

  private _data: any[];

  @Input() columns: DataColumn[] = [];
  @Input() pageSize: number = 10;
  @Output() selectedRow = new EventEmitter();

  dataToDisplay: any[] = [];
  pageNo: number = 1;
  pages: any[] = [];
  page: number = 1;
  searchText: string = '';

  constructor() {
  }
 
  ngOnInit() {
  }

  @Input() 
  set data(data: any[]) {
    this._data = data;
    this.pageNo = Math.ceil(this._data.length / this.pageSize);
    this.createRange();
  }

  createRange(){
    for(var i = 1; i <= this.pageNo; i++){
       this.pages.push(i);
    }
  }

  setPageNo(item: number) {
    this.page = item;
  }

  rowSelected(row) {
    this.selectedRow.emit(row);
  }
}