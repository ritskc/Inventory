import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CustomerService } from '../../admin/customer/customer.service';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { Customer } from '../../models/customer.model';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ToastrManager } from 'ng6-toastr-notifications';
import { ClassConstants } from '../../common/constants';
import { ShipmentService } from '../shipment.service';
import { MasterShipment } from '../../models/master.shipment.model';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { AppConfigurations } from '../../config/app.config';

@Component({
  selector: 'app-master-shipment-list',
  templateUrl: './master-shipment-list.component.html',
  styleUrls: ['./master-shipment-list.component.scss']
})
export class MasterShipmentListComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  private packagingSlipCreated: Subject<string> = new Subject<string>();
  customers: Customer[] = [];
  columns: DataColumn[] = [];
  customerId: number = -1;
  masterShipments: MasterShipment[] = [];
  filteredMasterShipments: MasterShipment[] = [];

  constructor(private companyService: CompanyService, private customerService: CustomerService, private httpLoader: httpLoaderService,
              private toastr: ToastrManager, private shipmentService: ShipmentService, private router: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllCustomers();
  }

  initializeGridColumns() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true, customStyling: 'column-width-150' }) );
    this.columns.push( new DataColumn({ headerText: "Master Slip No", value: "masterPackingSlipNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Packing Slips", value: "packingSlipNumbers", sortable: false, customStyling: 'column-width-100' }) );
    this.columns.push( new DataColumn({ headerText: "Updated Date", value: "updatedDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Traking Number", value: "trakingNumber", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "POS Uploaded", value: "isPOSUploaded", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Comment", value: "comment" }) );
    this.columns.push( new DataColumn({ headerText: "Action", isActionColumn: true, actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Warning, event: 'editMasterPackingSlip', icon: 'fa fa-edit' }),
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Primary, event: 'printBLForMasterPackingSlip', icon: 'fa fa-print' }),
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Danger, event: 'removeMasterPackingSlip', icon: 'fa fa-trash' })
    ] }) );
  }

  loadAllCustomers() {
    this.httpLoader.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe((customers) => {
          this.customers = customers;
          this.loadAllMasterShipments();
        },
        (error) => this.toastr.errorToastr(error),
        () => { this.httpLoader.hide(); });
  }

  loadAllMasterShipments() {
    this.httpLoader.show();
    this.shipmentService.getAllMasterShipments(this.currentlyLoggedInCompany)
        .subscribe((masterShipments) => {
          this.masterShipments = masterShipments;
          this.filteredMasterShipments = masterShipments;
          this.masterShipments.forEach(masterShipment => {
            masterShipment.customerName = this.customers.find(c => c.id == masterShipment.customerId).name;
            masterShipment.packingSlipNumbers = '';
            masterShipment.packingSlips.forEach((packingSlip) => {
              masterShipment.packingSlipNumbers += packingSlip.packingSlipNo + ', ';
            });
          });
        },
        (error) => this.toastr.errorToastr(error.error),
        () => { this.httpLoader.hide(); });
  }

  filterByCustomer() {
    this.filteredMasterShipments = this.customerId > 0 ? this.masterShipments.filter(s => s.customerId == this.customerId): this.masterShipments;
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'removeMasterPackingSlip':
        this.removeMasterPackingSlip(data);
        break;
      case 'editMasterPackingSlip':
        this.editMasterShipment(data);
        break;
      case 'printBLForMasterPackingSlip':
        this.printBLForMasterPackingSlip(data);
        break;
    }
  }

  addMasterShipment() {
    if (this.customerId < 0) {
      this.toastr.warningToastr('Please select the customer to create master packaging slip');
      return;
    }
    this.router.navigateByUrl(`companies/master-shipment-detail/${ this.customerId }/0/0`);
  }

  editMasterShipment(data) {
    this.router.navigateByUrl(`companies/master-shipment-detail/${ data.customerId }/1/${data.id}`);
  }

  printBLForMasterPackingSlip(data) {
    let appConfig = new AppConfigurations();
    this.packagingSlipCreated.next(`${appConfig.reportsUri}/MasterBL.aspx?id=${data.id}`);
  }

  removeMasterPackingSlip(data) {
    this.shipmentService.removeMasterPackingSlip(data.id)
        .subscribe(() => {
          this.toastr.successToastr('Master Packing Slip removed successfully!');
          this.loadAllMasterShipments();
        });
  }
}