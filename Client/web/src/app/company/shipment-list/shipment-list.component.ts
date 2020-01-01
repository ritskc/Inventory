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

  constructor(private companyService: CompanyService, private shipmentService: ShipmentService, private customerService: CustomerService, 
    private router: Router, private httpLoader: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllCustomers();
  }

  initializeGridColumns() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: false, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Crates", value: "crates", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Boxes", value: "boxes", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "isInvoiceCreated", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Payment", value: "isPaymentReceived", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "POS", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'downloadPOS', icon: 'fa fa-download' })
    ] }) );
  }



  initializeGridColumnsForDetails() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: false, isDate: true }) );
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
          this.filteredShipments = shipments;
        }, (error) => this.toastr.errorToastr(error),
        () => this.httpLoader.hide());
  }

  addShipment() {
    if (this.customerId > 0)
      this.router.navigateByUrl(`/companies/create-shipment/${ this.customerId }`);
    else 
      this.toastr.errorToastr('Please select the customer to proceed with shipment creation');
  }

  filterByCustomer(event) {
    let selectedCustomer = parseInt(event.target.value);
    if (selectedCustomer == -1) {
      this.filteredShipments = this.shipments;
      return;
    }
    this.filteredShipments = this.shipments.filter(s => s.customerId === selectedCustomer);
    this.showFullOrderDetails();
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'downloadPOS':
        this.downloadPOS(data);
        break;
    }
  }

  downloadPOS(data) {
    if (data.posPath) {
      //https://questapi.yellow-chips.com/File/POS/26/123456
      window.open(`${this.configuration.fileApiUri}/POS/${data.id}`);
    } else {
      this.toastr.errorToastr('POS is not uploaded for this shipment');
    }
  }

  showFullOrderDetails() {
    if (this.showFullDetails) {
      this.initializeGridColumnsForDetails();
      var shipmentsList = this.filteredShipments;
      this.filteredShipments = [];
      shipmentsList.forEach(shipment => {
        shipment.packingSlipDetails.forEach(detail => {
          var viewModel = new ShipmentListViewModel();
          viewModel.customerName = shipment.customerName;
          viewModel.packingSlipNo = shipment.packingSlipNo;
          viewModel.shippingDate = shipment.shippingDate;
          viewModel.orderNo = detail.orderNo;
          viewModel.partCode = detail.partDetail? detail.partDetail.partCode: '';
          viewModel.partDescription = detail.partDetail? detail.partDetail.partDetail: '';
          viewModel.shippingDate = detail.qty;
          this.filteredShipments.push(viewModel);
        });
      });
    } else {
      this.initializeGridColumns();
      this.filteredShipments = this.shipments;
    }
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