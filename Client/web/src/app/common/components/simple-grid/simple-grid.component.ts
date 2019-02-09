import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChange } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';

@Component({
  selector: 'simple-grid',
  templateUrl: './simple-grid.component.html',
  styleUrls: ['./simple-grid.component.scss']
})
export class SimpleGridComponent implements OnInit, OnChanges {

  private _data: any[];

  @Input() addRequired: boolean = true;
  @Input() exportRequired: boolean = true;
  @Input() columns: DataColumn[] = [];
  @Input() pageSize: number = 10;
  @Output() selectedRow = new EventEmitter();
  @Output() addClickedEventEmitter = new EventEmitter();

  dataToDisplay: any[] = [];
  pageNo: number = 1;
  pages: any[] = [];
  page: number = 1;
  searchText: string = '';

  constructor() {
  }
 
  ngOnInit() {

  }

  ngOnChanges(changes: {[propKey: string]: SimpleChange}) {
    
  }

  @Input() 
  set data(data: any[]) {
    this._data = data;
    this.calculatePages();
    this.createRange();
  }

  calculatePages() {
    this.pageNo = Math.ceil(this._data.length / this.pageSize);
  }

  createRange(){
    this.pages = [];
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

  pageSizeSelected() {
    this.calculatePages();
    this.createRange();
  }

  dataChanges() {
    this.calculatePages();
    this.createRange();
  }

  addClicked() {
    this.addClickedEventEmitter.emit(true);
  }
}