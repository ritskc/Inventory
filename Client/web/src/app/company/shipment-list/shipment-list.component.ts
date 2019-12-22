import { Component, OnInit } from '@angular/core';
import { DataColumn } from '../../models/dataColumn.model';
import { CompanyService } from '../company.service';
import { Router } from '@angular/router';
import { ShipmentService } from '../shipment.service';
import { Shipment } from '../../models/shipment.model';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer } from '../../models/customer.model';
import { map, mergeMap } from 'rxjs/operators';

@Component({
  selector: 'app-shipment-list',
  templateUrl: './shipment-list.component.html',
  styleUrls: ['./shipment-list.component.scss']
})
export class ShipmentListComponent implements OnInit {

  private currentlyLoggedInCompany: number = 0;
  shipments: Shipment[] = [];
  customers: Customer[] = [];
  columns: DataColumn[] = [];

  constructor(private companyService: CompanyService, private shipmentService: ShipmentService, private customerService: CustomerService, private router: Router) { }

  ngOnInit() {
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllCustomers();
  }

  initializeGridColumns() {
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: false, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Crates", value: "crates", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Boxes", value: "boxes", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "isInvoiceCreated", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Payment", value: "isPaymentReceived", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
  }

  loadAllCustomers() {
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
        });
  }

  addShipment() {
    this.router.navigateByUrl(`/companies/create-shipment`);
  }
}