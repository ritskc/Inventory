import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../company.service';
import { CustomerService } from '../../../admin/customer/customer.service';
import { ShipmentService } from '../../shipment.service';
import { Customer } from '../../../models/customer.model';
import { Shipment } from '../../../models/shipment.model';
import { FileUploadService } from '../../../common/services/file-upload.service';
import { ToastrManager } from 'ng6-toastr-notifications';
import { pipe } from 'rxjs';
import { httpLoaderService } from '../../../common/services/httpLoader.service';

@Component({
  selector: 'pos-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class POSUploadComponent implements OnInit {

  private file: FileList;
  private customerId: number = -1;
  private selection: number = 1;
  private currentlyLoggedInCompany: number = -1;
  private data: any;
  private shipments: Shipment[] = [];
  private customers: Customer[] = [];
  private tracking: string = '';
  private shipmentId: number = 0;
  
  constructor(private companyService: CompanyService, private customerService: CustomerService, private loaderService: httpLoaderService,
              private shipmentService: ShipmentService, private fileService: FileUploadService, private toastr: ToastrManager
    ) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
  }

  loadAllCustomers() {
    this.loaderService.show();
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe((customer) => this.customers = customer,
        (error) => console.log(error),
        () => this.loaderService.hide());
  }

  uploadSelected(event) {
    this.shipments = [];
    this.data = [];
    this.tracking = '';
    this.selection = 1;
  }

  masterSelected(event) {
    this.shipments = [];
    this.data = [];
    this.tracking = '';
    this.selection = 3;
  }

  deleteSelected(event) {
    this.selection = 2;
  }

  customerSelected() {
    this.loaderService.show();
    this.shipments = [];
    if (this.selection == 1) {
      this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe((shipments) => {
          this.data = shipments.filter(s => s.customerId == this.customerId && !s.isMasterPackingSlip && !s.isPOSUploaded);
          this.shipmentId = -1;
          //this.data = this.shipments;
        }, (error) => console.log(error)
        ,() => this.loaderService.hide());
    } else if (this.selection == 3) {
      this.shipmentService.getAllMasterShipments(this.currentlyLoggedInCompany)
        .subscribe((shipments) => {
          this.data = shipments.filter(s => s.customerId == this.customerId && !s.isPOSUploaded);
          this.shipmentId = -1;
        }, (error) => console.log(error)
        ,() => this.loaderService.hide());
    }
  }

  uploadFile(file: FileList) {
    this.file = file;
  }

  upload() {
    if (this.shipmentId > 0) {
      var trackingNumber = this.tracking? this.tracking: '_';
      var item = this.selection == 1 ? {'type': 'POS', 'file': this.file[0]}: {'type': 'MasterPOS', 'file': this.file[0]};
      this.fileService.uploadFile(item, `${this.shipmentId}/${ trackingNumber }`);
      this.toastr.successToastr('Document uploaded successfully!!');
      this.customerId = -1;
      this.shipmentId = -1;
      this.tracking = '';
    }
  }
}
