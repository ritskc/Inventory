import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { httpLoaderService } from '../../../common/services/httpLoader.service';
import { CompanyService } from '../../../company/company.service';
import { Container } from '../../../models/container.model';
import { UserAction } from '../../../models/enum/userAction';
import { ContainerService } from '../container.service';
import * as DateHelper from '../../../common/helpers/dateHelper';
import { InvoiceService } from '../../invoice/invoice.service';
import { Invoice } from '../../../models/invoice.model';
import { DataColumn, DataColumnAction } from '../../../models/dataColumn.model';
import { ClassConstants } from '../../../common/constants';
import { ToastrManager } from 'ng6-toastr-notifications';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})
export class ContainerEditComponent implements OnInit {

  eta: string;
  containerId: number = 0;
  invoiceId: number = -1;
  formMode: UserAction;
  container: Container = new Container();
  columns: DataColumn[] = [];
  invoices: Invoice[] = [];

  constructor(private activatedRoute: ActivatedRoute, private router: Router, private loaderService: httpLoaderService, private toastr: ToastrManager,
              private companyService: CompanyService, private invoiceService: InvoiceService, private containerService: ContainerService) { }

  ngOnInit() {
    this.formMode = this.activatedRoute.snapshot.params.mode == 0? UserAction.Add: UserAction.Edit;
    this.containerId = this.activatedRoute.snapshot.params.containerId;

    this.columns.push( new DataColumn({ headerText: "Invoice No", value: "invoiceNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", isDate: true, sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "ETA", value: "eta", isDate: true, sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, icon: 'fa fa-trash', event: 'deleteInvoice' })
    ] }) );
    
    if (this.formMode == UserAction.Add)
      this.container = new Container();
    else {
        this.loaderService.show();
        this.containerService.getAContainer(this.containerId)
            .subscribe(
              (container) => {
                this.container = container;
                this.eta = DateHelper.formatDate(new Date(container.eta));
              },
              (error) => console.log(error),
              () => { }
            );
      }
    
    this.invoiceService.getAllInvoices(this.companyService.getCurrentlyLoggedInCompanyId())
        .subscribe(
          (invoices) => {
            this.invoices = invoices.filter(i => i.isInvoiceReceived == false);
            console.log(this.invoices);
          },
          (error) => console.log(error),
          () => this.loaderService.hide()
        );
  }

  addToContainer() {
    if (this.invoiceId < 1) {
      this.toastr.errorToastr('Please select an invoice to add');
      return;
    }

    var invoiceToAdd = this.invoices.find(i => i.id == this.invoiceId);
    this.container.supplierInvoices.push(invoiceToAdd);
  }

  actionButtonClicked(data) {

    switch (data.eventName) {
      case 'deleteInvoice':
        var index = this.container.supplierInvoices.findIndex(i => i.id == data.id);
        this.container.supplierInvoices.splice(index, 1);
        break;
    }

  }

  saveContainer() {
    if (!this.container.containerNo) {
      this.toastr.warningToastr('Container number is mandatory');
      return;
    }

    if (this.container.supplierInvoices.length == 0) {
      this.toastr.warningToastr('Please select at least one invoice to the container');
      return;
    }

    this.loaderService.show();
    this.container.companyId = this.companyService.getCurrentlyLoggedInCompanyId();
    this.container.eta = this.eta;
    this.containerService.saveContainer(this.container)
        .subscribe(
          (result) => {
            console.log(result);
            this.toastr.successToastr('Container saved successfully');
            this.router.navigateByUrl(`invoice/container/list`);
          },
          (error) => {this.toastr.errorToastr(error.error); this.loaderService.hide();},
          () => this.loaderService.hide()
        );
  }
}