import { Component, OnInit } from '@angular/core';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../../common/constants';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { Container } from '../../../models/container.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { ContainerService } from '../container.service';
import * as DateHelper from '../../../common/helpers/dateHelper';
import { Router } from '@angular/router';
import { Warehouse } from '../../../models/company.model';
import { CompanyService } from '../../../company/company.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ContainerListComponent implements OnInit {

  showNotReceived: boolean = false;
  showWarehouseModal: boolean = false;
  warehouseId: number = 0;
  dataToUpdate: Container;
  warehouses: Warehouse[] = [];
  columns: DataColumn[] = [];
  containers: Container[] = [];

  constructor(private containerService: ContainerService, private router: Router, private companyService: CompanyService,
              private loaderService: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.populateColumnsForContainerList();
    this.getAllConatinerList();
  }

  populateColumnsForContainerList() {
    this.columns.push( new DataColumn({ headerText: "Container No", value: "containerNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoices", value: "invoices", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", isDate: true, sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Received On", value: "receivedDate", isDate: true, sortable: true, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Received", value: "isContainerReceived", isDisabled: true, isBoolean: true, customStyling: 'center' }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editContainer' }),
      new DataColumnAction({ actionText: 'Receive', actionStyle: ClassConstants.Primary, event: 'receiveContainer', showOnlyIf: 'data["isContainerReceived"] == false' }),
      new DataColumnAction({ actionText: 'Un Receive', actionStyle: ClassConstants.Primary, event: 'unreceiveContainer', showOnlyIf: 'data["isContainerReceived"] == true' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'deleteContainer' })
    ] }) );
  }

  getAllConatinerList() {
    this.containers = [];
    this.loaderService.show();
    this.containerService.getAllContainersInACompany()
        .subscribe(
          (containers) => {
            containers = this.showNotReceived ? containers.filter(c => c.isContainerReceived == !this.showNotReceived): containers;
            this.formatData(containers);
          },
          (error) => console.log(error),
          () => this.loaderService.hide()
        );
  }

  formatData(containers: Container[]) {
    containers.forEach(container => {
      container.invoices = '';
      container.invoiceDate = '';
      container.supplierInvoices.forEach(invoice => {
        container.invoices += `${ invoice.invoiceNo }, `;
        container.invoiceDate += `${ DateHelper.formatDate(new Date(invoice.invoiceDate)) }, `;
      });
      this.containers.push(container);
    });
  }

  actionButtonClicked(data) {
    switch (data.eventName) {

      case 'receiveContainer':
        this.dataToUpdate = data;
        this.companyService.getCompany(this.companyService.getCurrentlyLoggedInCompanyId())
            .subscribe(company => {
              if (company.warehouses && company.warehouses.length > 0) {
                this.warehouses = company.warehouses;
                this.showWarehouseModal = true;
              } else {
                this.receiveOrderInWarehouse();
              }
            });
        break;

      case 'unreceiveContainer':
        this.loaderService.show();
        this.containerService.unreceiveContainer(data.id)
            .subscribe(() => {
              this.toastr.successToastr('Container unreceived successfully!!');
              data.isContainerReceived = false;
            }, 
              (error) => { this.toastr.errorToastr(error.error); this.loaderService.hide(); },
              () => this.loaderService.hide()
            );
        break;

      case 'editContainer':
        this.router.navigateByUrl(`invoice/container/edit/1/${ data.id }`);
        break;

      case 'deleteContainer':
        this.deleteContainer(data)
        break;
    }
  }

  addContainer() {
    this.router.navigateByUrl(`invoice/container/edit/0/0`);
  }

  filterOptionSelected() {
    this.getAllConatinerList();
  }

  receiveOrderInWarehouse() {
    this.showWarehouseModal = false;
    this.loaderService.show();
        this.containerService.receiveContainer(this.dataToUpdate.id, this.warehouseId)
            .subscribe(() => {
              this.toastr.successToastr('Container received to the selected or default warehouse successfully');
              this.dataToUpdate.isContainerReceived = true;
            }, 
            (error) => { this.toastr.errorToastr(error.error); this.loaderService.hide(); },
            () => this.loaderService.hide()
          );
  }

  deleteContainer(data: Container) {
    if (data.isContainerReceived) {
      this.toastr.warningToastr('Received containers cannot be removed.');
      return;
    }
    var response = confirm('Are you sure you want to remove this container?');
    if (response) {
      this.containerService.deleteContainer(data.id)
          .subscribe(
            () => {
              this.toastr.successToastr('Container deleted successfully');
              this.getAllConatinerList();
            },
            (error) => this.toastr.errorToastr(error.error)
          );
    }
  }
}