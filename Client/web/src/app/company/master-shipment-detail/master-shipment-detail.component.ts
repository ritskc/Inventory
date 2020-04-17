import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { ShipmentService } from '../shipment.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrManager } from 'ng6-toastr-notifications';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { UserAction } from '../../models/enum/userAction';
import * as DateHelper from '../../common/helpers/dateHelper';
import { MasterShipment } from '../../models/master.shipment.model';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer } from '../../models/customer.model';
import { Shipment } from '../../models/shipment.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ClassConstants } from '../../common/constants';

@Component({
  selector: 'app-master-shipment-detail',
  templateUrl: './master-shipment-detail.component.html',
  styleUrls: ['./master-shipment-detail.component.scss']
})
export class MasterShipmentDetailComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private userAction: UserAction;
  private masterShipment: MasterShipment = new MasterShipment();
  private selectedCustomer: Customer;
  private shipments: Shipment[] = [];
  private columns: DataColumn[] = [];
  private selectedShipmentId: number = -1;

  constructor(private companyService: CompanyService, private shipmentService: ShipmentService, private router: Router, private customerService: CustomerService,
              private toastr: ToastrManager, private httpLoaderService: httpLoaderService, private activatedRouter: ActivatedRoute) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.userAction = this.activatedRouter.snapshot.params.mode == 0? UserAction.Add: UserAction.Edit;
    
    this.initializeGridColumns();
    this.getSelectedCustomerDetail();
    this.getShipmentsForMasterShipmentCreation();

    if (this.userAction === UserAction.Add) 
      this.prepareScreenForMasterShipmentCreate();
    else
      this.prepareScreenForMasterShipmentUpdate();
  }

  initializeGridColumns() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: false, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Invoice Date", value: "invoiceDate", sortable: false, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "isInvoiceCreated", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Payment", value: "isPaymentReceived", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Actions", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'removeShipment', icon: 'fa fa-trash' })
    ] }) );
  }

  getSelectedCustomerDetail() {
    this.customerService.getCustomer(this.currentlyLoggedInCompany, this.activatedRouter.snapshot.params.customerId)
        .subscribe(customer => {
          this.selectedCustomer = customer;
          this.masterShipment.customerId = this.selectedCustomer.id;
        });
  }

  getShipmentsForMasterShipmentCreation() {
    this.httpLoaderService.show();
    this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe(shipments => {
          this.shipments = shipments.filter(s => s.customerId == this.selectedCustomer.id && !s.isPOSUploaded);
        }, (error) => this.toastr.errorToastr('Error while fetching shipments'),
        () => this.httpLoaderService.hide());
  }

  prepareScreenForMasterShipmentCreate() {
    this.masterShipment.companyId = this.currentlyLoggedInCompany;

    this.shipmentService.getLatestMasterShipment(this.currentlyLoggedInCompany, DateHelper.getToday()).subscribe(data => {
      this.masterShipment.masterPackingSlipNo = data ? data.entityNo : 'Data Unavailable';
    });
  }

  prepareScreenForMasterShipmentUpdate() {
    this.httpLoaderService.show();

    this.shipmentService.getAMasterShipments(this.currentlyLoggedInCompany, this.activatedRouter.snapshot.params.shipmentId)
        .subscribe(masterShipment => {
          this.masterShipment = masterShipment;
        }, 
        (error) => this.toastr.errorToastr(error.error),
        () => this.httpLoaderService.hide());
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'removeShipment':
        let index = this.masterShipment.packingSlips.findIndex(s => s.id == data.id);
        this.masterShipment.packingSlips.splice(index, 1);
        break;
    }
  }

  shipmentSelected(event) {
    if (this.masterShipment.packingSlips.some(s => s.id == this.selectedShipmentId)) {
      this.toastr.warningToastr('This selected shipment already added to the list');
      return;
    } else if (this.selectedShipmentId == -1) {
      return;
    }

    let selectedShipment = this.shipments.find(s => s.id == this.selectedShipmentId);
    this.masterShipment.packingSlips.push(selectedShipment);
  }

  save() {
    if (this.masterShipment.packingSlips.length === 0) {
      this.toastr.warningToastr('At least one shipment should be present to save');
      return;
    }
    this.httpLoaderService.show();
    this.masterShipment.updatedDate = DateHelper.getToday();
    this.shipmentService.saveMasterShipment(this.masterShipment)
        .subscribe(() => {
          this.toastr.successToastr('Master shipment updated successfully');
          this.back();
        }, (error) => this.toastr.errorToastr('Unable to save master shipment'),
        () => this.httpLoaderService.hide());
  }

  back() {
    this.router.navigateByUrl('companies/master-shipment-list');
  }

  remove() {
    if (!confirm('Are you sure to remove this master packaging slip?')) {
      return;
    }

    this.shipmentService.removeMasterPackingSlip(this.activatedRouter.snapshot.params.shipmentId)
        .subscribe(() => this.back());
  }
}