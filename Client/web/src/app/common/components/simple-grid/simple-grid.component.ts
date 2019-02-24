import { Component, OnInit, Input, Output, EventEmitter, OnChanges, SimpleChange } from '@angular/core';
import { DataColumn } from '../../../models/dataColumn.model';
import { Utils } from '../../utils/utils';
import { JsonToCsvExporterService } from '../../services/json-to-csv-exporter.service';

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
  @Input() defaultSortColumnName: string = 'name';
  @Output() selectedRow = new EventEmitter();
  @Output() addClickedEventEmitter = new EventEmitter();

  dataToDisplay: any[] = [];
  pageNo: number = 1;
  pages: any[] = [];
  page: number = 1;
  searchText: string = '';
  ascendingSortOrder: boolean = true;
  currentSortColumnName: string = '';

  constructor(private jsonToCsvExporter: JsonToCsvExporterService) {
  }
 
  ngOnInit() {
    this.currentSortColumnName = this.defaultSortColumnName;
  }

  ngOnChanges(changes: {[propKey: string]: SimpleChange}) {
    
  }

  @Input() 
  set data(data: any[]) {
    this._data = Utils.sortArray(data, this.defaultSortColumnName, true);
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
    this.page = 1;
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

  toggleSort(column: DataColumn) {
    if (!column.sortable) return;
    
    this.ascendingSortOrder = !this.ascendingSortOrder;
    this.currentSortColumnName = column.value;
    this._data = Utils.sortArray(this._data, column.value, this.ascendingSortOrder);
    this.pageSizeSelected();
  }

  isNotSortedOnThisColumn(columnName: string) {
    return columnName != this.currentSortColumnName;
  }

  descendingOrderSelected(columnName: string) {
    return columnName == this.currentSortColumnName && !this.ascendingSortOrder;
  }

  export() {
    this.jsonToCsvExporter.export(`Company Details ${Date.now()}`, 'csv', this._data);
  }
}