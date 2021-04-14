import { Component, OnInit } from '@angular/core';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Container } from '../../../models/container.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { ContainerService } from '../container.service';
import * as DateHelper from '../../../common/helpers/dateHelper';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ContainerListComponent implements OnInit {

  columns: DataColumn[] = [];
  containers: Container[] = [];

  constructor(private containerService: ContainerService, 
              private loaderService: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.populateColumnsForContainerList();
    this.getAllConatinerList();
  }

  populateColumnsForContainerList() {
    this.columns.push( new DataColumn({ headerText: "Container No", value: "containerNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoices", value: "invoices", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", isDate: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Received Date", value: "receivedDate", isDate: true, sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Received", value: "isContainerReceived", isDisabled: true, isBoolean: true, customStyling: 'center' }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Receive', actionStyle: ClassConstants.Primary, event: 'receiveContainer' }),
      new DataColumnAction({ actionText: 'Un Receive', actionStyle: ClassConstants.Primary, event: 'unreceiveContainer' }),
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Primary, event: 'editContainer' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteContainer' })
    ] }) );
  }

  getAllConatinerList() {
    this.loaderService.show();
    this.containerService.getAllContainersInACompany()
        .subscribe(
          (containers: Container[]) => this.formatData(containers),
          (error) => console.log(error),
          () => this.loaderService.hide()
        );
  }

  formatData(containers: Container[]) {
    containers.forEach(container => {
      container.invoices = '';
      container.invoiceDate = '';
      container.supplierInvoices.forEach(invoice => {
        container.invoices += `${ invoice.invoiceNo },`;
        container.invoiceDate += `${ DateHelper.formatDate(new Date(invoice.invoiceDate)) }, `;
      });
      this.containers.push(container);
    });
  }
}