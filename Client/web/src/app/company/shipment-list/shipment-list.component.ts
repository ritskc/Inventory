import { Component, OnInit } from '@angular/core';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { ShipmentService } from '../shipment.service';
import { Shipment } from '../../models/shipment.model';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer } from '../../models/customer.model';
import { map, mergeMap } from 'rxjs/operators';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../common/constants';
import { AppConfigurations } from '../../config/app.config';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-shipment-list',
  templateUrl: './shipment-list.component.html',
  styleUrls: ['./shipment-list.component.scss']
})
export class ShipmentListComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private configuration: AppConfigurations = new AppConfigurations();
  shipments: Shipment[] = [];
  filteredShipments: any[] = [];
  customers: Customer[] = [];
  columns: DataColumn[] = [];
  customerId: number = -1;
  showFullDetails: boolean = false;
  showInvoiced: boolean = false;
  showRepackge: boolean = false;
  printDocument: Subject<string> = new Subject<string>();

  constructor(private companyService: CompanyService, private shipmentService: ShipmentService, private customerService: CustomerService, 
    private router: Router, private httpLoader: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllCustomers();
  }

  initializeGridColumns() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true, customStyling: 'column-width-200' }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Tracking", value: "trakingNumber", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "isInvoiceCreated", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Payment", value: "isPaymentReceived", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "POS", value: "isPOSUploaded", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editShipment' }),
      new DataColumnAction({ actionText: 'Invoice', actionStyle: ClassConstants.Primary, event: 'editInvoice' }),
      new DataColumnAction({ actionText: 'Repack', actionStyle: ClassConstants.Primary, event: 'printRepackingInvoice', showOnlyIf: 'data["isRepackage"] == true' }),
      new DataColumnAction({ actionText: 'Print Shipment', actionStyle: ClassConstants.Primary, event: 'printShipment' }),
      new DataColumnAction({ actionText: 'Print Invoice', actionStyle: ClassConstants.Primary, event: 'printInvoice' }),
      new DataColumnAction({ actionText: 'Print BL', actionStyle: ClassConstants.Primary, event: 'printBL' }),
      new DataColumnAction({ actionText: 'Download POS', actionStyle: ClassConstants.Primary, event: 'downloadPOS' }),
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'delete' })
    ] }) );
  }

  initializeGridColumnsForDetails() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Order No", value: "orderNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Part Code", value: "partCode", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Description", value: "partDescription", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Quantity", value: "shippedQty", sortable: false, customStyling: 'right' }) );
  }

  loadAllCustomers() {
    this.httpLoader.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .pipe(
          map(customers => {
            this.customers = customers;
            return customers;
          }),
          mergeMap(customers => this.shipmentService.getAllShipments(this.currentlyLoggedInCompany))
        ).subscribe(shipments => {
          shipments.map(shipment => {
            var customer = this.customers.find(c => c.id === shipment.customerId);
            if (customer)
              shipment.customerName = customer.name;
          });
          this.shipments = shipments;
          this.filteredShipments = this.customerId > 0 ? this.shipments.filter(s => s.customerId == this.customerId && !s.isMasterPackingSlip): this.shipments;
        }, (error) => this.toastr.errorToastr(error),
        () => this.httpLoader.hide());
  }

  addShipment() {
    if (this.customerId > 0)
      this.router.navigateByUrl(`/companies/create-shipment/${ this.customerId }/0/0`);
    else 
      this.toastr.warningToastr('Please select the customer to proceed with shipment creation');
  }

  filterByCustomer(event) {
    if (this.customerId == -1) {
      this.filteredShipments = this.shipments;
      return;
    }
    this.showFullOrderDetails();
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'downloadPOS':
        this.downloadPOS(data);
        break;
      case 'printInvoice':
        if (!data.isInvoiceCreated) {
          this.toastr.warningToastr('Cannot be printed since the invoice is not yet created!!');
          return;
        }
        this.print('invoice', data);
        break;
      case 'printShipment':
        this.print('shipment', data);
        break;
      case 'printRepackingInvoice':
        this.print('RepackingInvoice', data);
        break;
      case 'printBL':
        if (data.isMasterPackingSlip) {
          this.print('MasterBL', data);
        } else {
          this.print('BL', data);
        }
        break;
      case 'delete':
        this.delete(data);
        break;
      case 'editShipment':
        this.editShipment(data);
        break;
      case 'editInvoice':
        this.editInvoice(data);
        break;
    }
  }

  downloadPOS(data) {
    if (data.isMasterPackingSlip && data.masterPackingSlipId > 0) {
      window.open(`${this.configuration.fileApiUri}/MasterPOS/${data.masterPackingSlipId}`);
    }
    else if (data.posPath) {
      window.open(`${this.configuration.fileApiUri}/POS/${data.id}`);
    } else {
      this.toastr.warningToastr('POS is not uploaded for this shipment');
    }
  }

  editShipment(data) {
    this.router.navigateByUrl(`companies/create-shipment/${data.customerId}/1/${data.id}`);
  }

  editInvoice(data) {
    if (data.isInvoiceCreated) {
      this.toastr.warningToastr('This cannot be edited since it is already invoiced');
      return;
    }
    this.router.navigateByUrl(`companies/invoice/${data.customerId}/${data.id}`);
  }

  print(type: string, data: any) {
    var appConfig = new AppConfigurations();
    if (type === 'shipment') {
      this.printDocument.next(`${appConfig.reportsUri}/PackingSlip.aspx?id=${data.id}`);
    } else if (type === 'invoice') {
      this.printDocument.next(`${appConfig.reportsUri}/Invoice.aspx?id=${data.id}`);
    } else if (type === 'BL') {
      this.printDocument.next(`${appConfig.reportsUri}/BL.aspx?id=${data.id}`);
    } else if (type === 'MasterBL') {
      this.printDocument.next(`${appConfig.reportsUri}/MasterBL.aspx?id=${data.masterPackingSlipId}`);
    } else if (type === 'RepackingInvoice') {
      this.printDocument.next(`${appConfig.reportsUri}/RepackingInvoice.aspx?id=${data.id}`);
    }
  }

  delete(data: any) {
    var confirmation = confirm('Are you sure you want to remove this shipment?');
    if (!confirmation) 
      return;

    this.httpLoader.show();
    this.shipmentService.deleteShipment(data.id)
        .subscribe(
          () => {
            this.toastr.successToastr('Shipment removed successfully!!');
            this.loadAllCustomers();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoader.hide();
          },
          () => { this.httpLoader.hide(); }
        );
  }

  showFullOrderDetails() {
    if (this.showFullDetails) {
      this.initializeGridColumnsForDetails();
      var shipmentsList = this.customerId > 0 ? this.shipments.filter(s => s.customerId == this.customerId): this.shipments;
      this.filteredShipments = [];
      shipmentsList.forEach(shipment => {
        shipment.packingSlipDetails.forEach(detail => {
          var viewModel = new ShipmentListViewModel();
          viewModel.customerName = shipment.customerName;
          viewModel.packingSlipNo = shipment.packingSlipNo;
          viewModel.shippingDate = shipment.shippingDate;
          viewModel.orderNo = detail.orderNo;
          viewModel.partCode = detail.partDetail? detail.partDetail.code: '';
          viewModel.partDescription = detail.partDetail? detail.partDetail.description: '';
          viewModel.shippedQty = detail.qty;
          this.filteredShipments.push(viewModel);
        });
      });
    } else {
      this.initializeGridColumns();
      this.filteredShipments = this.customerId > 0 ? this.shipments.filter(s => s.customerId == this.customerId): this.shipments;
    }
    this.showInvoiced = false;
    this.showRepackge = false;
  }

  showInvoicedOnly() {
    if (this.showInvoiced) {
      this.filteredShipments = this.shipments.filter(i => i.isInvoiceCreated == true);
    } else {
      this.filteredShipments = this.shipments;
    }
  }

  showRepackagedOnly() {
    this.filteredShipments = this.showRepackge ? this.shipments.filter(i => i.isRepackage == true): this.shipments;
  }
}

class ShipmentListViewModel {
  customerName: string;
  packingSlipNo: string;
  shippingDate: string;
  orderNo: string;
  partCode: string;
  partDescription: string;
  shippedQty: number;
}