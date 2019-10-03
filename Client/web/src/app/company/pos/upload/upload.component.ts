import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../../company.service';
import { CustomerService } from '../../../admin/customer/customer.service';
import { ShipmentService } from '../../shipment.service';
import { Customer } from '../../../models/customer.model';
import { Shipment } from '../../../models/shipment.model';

@Component({
  selector: 'pos-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class POSUploadComponent implements OnInit {

  private customerId: number = -1;
  private selection: number = 1;
  private currentlyLoggedInCompany: number = -1;
  private shipments: Shipment[] = [];
  private customers: Customer[] = [];
  
  constructor(private companyService: CompanyService, private customerService: CustomerService,
              private shipmentService: ShipmentService
    ) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.loadAllCustomers();
  }

  loadAllCustomers() {
    this.customerService.getAllCustomers(this.currentlyLoggedInCompany)
        .subscribe((customer) => this.customers = customer);
  }

  customerSelected() {
    this.shipments = [];
    this.shipmentService.getAllShipments(this.currentlyLoggedInCompany)
        .subscribe((shipments) => {
          this.shipments = shipments.filter(s => s.customerId == this.customerId && !s.isPaymentReceived);
        });
  }

  uploadSelected(event) {
    this.selection = 1;
  }

  deleteSelected(event) {
    this.selection = 2;
  }
}
