import { Component, OnInit } from '@angular/core';
import { InvoiceService } from '../../admin/invoice/invoice.service';
import { CompanyService } from '../company.service';
import { UserAction } from '../../models/enum/userAction';
import { ActivatedRoute, Router } from '@angular/router';
import * as DateHelper from '../../common/helpers/dateHelper';
import { httpLoaderService } from '../../common/services/httpLoader.service';
import { Shipment, PackingSlipDetail } from '../../models/shipment.model';
import { PartsService } from '../../admin/parts/parts.service';
import { Part } from '../../models/part.model';
import { ToastrManager } from 'ng6-toastr-notifications';
import { DataColumn, DataColumnAction } from '../../models/dataColumn.model';
import { ClassConstants } from '../../common/constants';
import { CustomerService } from '../../admin/customer/customer.service';
import { Customer } from '../../models/customer.model';

@Component({
  selector: 'app-edit-monthly-invoice',
  templateUrl: './edit-monthly-invoice.component.html',
  styleUrls: ['./edit-monthly-invoice.component.scss']
})
export class EditMonthlyInvoiceComponent implements OnInit {

  private selectedCustomerId: number = 0;
  private quantity: number = 0;
  private quantityInHand: number = 0;
  private unitPrice: number = 0;
  private shipment: Shipment = new Shipment();
  private mode: UserAction;
  private currentlyLoggedInCompany: number = 0;
  private previousMonthlyInvoiceNo: string = '';
  private shippingDate: string = '';
  private orderNo: string = '';
  private partId: number = 0;

  private cols: DataColumn[] = [];
  private parts: Part[] = [];
  private selectedPart: Part;
  private shipmentDetail: PackingSlipDetail = new PackingSlipDetail();

  constructor(private companyService: CompanyService, private invoiceService: InvoiceService, private activatedRoute: ActivatedRoute, private customerService: CustomerService,
              private httpLoaderService: httpLoaderService, private partService: PartsService, private toastr: ToastrManager, private route: Router) { }

  ngOnInit() {
    this.selectedCustomerId = this.activatedRoute.snapshot.params.customerId;
    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.mode = this.activatedRoute.snapshot.params.action == UserAction.Add ? UserAction.Add : UserAction.Edit;
    this.prepareForm();
    this.cols.push( new DataColumn({ headerText: "PO Number", value: "orderNo", isEditable: true, sortable: true }) );
    this.cols.push( new DataColumn({ headerText: "Part Code", value: "partCode", sortable: true }) );
    this.cols.push( new DataColumn({ headerText: "Description", value: "partDescription", sortable: true }) );
    this.cols.push( new DataColumn({ headerText: "Quantity", value: "qty", isEditable: true, sortable: false, customStyling: 'right column-width-50' }) );
    this.cols.push( new DataColumn({ headerText: "Unit Price", value: "unitPrice", sortable: false, isEditable: true, customStyling: 'right column-width-100' }) );
    this.cols.push( new DataColumn({ headerText: "Total", value: "price", sortable: false, customStyling: 'right' }) );
    this.cols.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: '', actionStyle: ClassConstants.Danger, event: 'removePart', icon: 'fa fa-trash' })
    ] }) );
  }

  private prepareForm() {
    this.httpLoaderService.show();
    this.partService.getAllParts(this.currentlyLoggedInCompany)
        .subscribe(
          (parts) => {
            this.parts = parts.filter(p => p.partCustomerAssignments.find(c => c.customerId == this.selectedCustomerId));
            this.httpLoaderService.hide();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoaderService.hide();
          }
        );
    if (this.mode == UserAction.Add) {
      this.shipment.CompanyId = this.currentlyLoggedInCompany;
      this.shipment.customerId = this.selectedCustomerId;
      this.shippingDate = DateHelper.getToday();
      this.customerService.getCustomer(this.currentlyLoggedInCompany, this.selectedCustomerId)
                          .subscribe((customer) => this.shipment.shipVia = customer.truckType);
      this.invoiceService.getPreviousMonthlyInvoiceNo(this.currentlyLoggedInCompany, DateHelper.getToday()).subscribe(data => {
        this.previousMonthlyInvoiceNo = data ? data.entityNo : 'Data Unavailable';
      });
    } else {
      this.httpLoaderService.show();
      this.invoiceService.getAMonthlyInvoice(this.currentlyLoggedInCompany, this.activatedRoute.snapshot.params.invoiceId)
          .subscribe(
            (shipment) => {
              this.shipment = shipment;
              this.shippingDate = DateHelper.formatDate(new Date(this.shipment.shippingDate));
              this.shipment.packingSlipDetails.forEach(detail => {
                detail.partCode = detail.partDetail.code;
                detail.partDescription = detail.partDetail.description;
              })
            },
            (error) => this.toastr.errorToastr('Error while loading monthly invoice')
          );
    }
  }

  partSelected() {
    this.quantity = 0;
    this.quantityInHand = 0;
    this.unitPrice = 0;

    this.selectedPart = this.parts.find(p => p.id == this.partId);
    this.shipmentDetail = new PackingSlipDetail();
    if (this.selectedPart) {
      this.unitPrice = this.selectedPart.partCustomerAssignments.find(c => c.customerId == this.selectedCustomerId).rate;
      this.quantityInHand = this.selectedPart.qtyInHand + this.selectedPart.openingQty;
    } else {
      this.selectedPart = new Part();
    }
  }

  valueChanged(data) {
    this.shipment.packingSlipDetails.forEach(detail => detail.price = parseFloat((detail.unitPrice * detail.qty).toFixed(2)));
  }

  actionButtonClicked(data) {
    switch(data.eventName) {
      case 'removePart':
        var index = this.shipment.packingSlipDetails.findIndex(d => d == data);
        this.shipment.packingSlipDetails.splice(index, 1);
        break;
    }
  }

  quantityChanged() {
    this.shipmentDetail.price = parseFloat((this.quantity * this.unitPrice).toFixed(2));
  }

  addPart() {
    if (this.partId < 1) {
      this.toastr.errorToastr('Please select a valid part to add');
      return;
    }

    if ((this.quantityInHand - this.quantity) < 0) {
      var response = confirm('Your inventory quantity is going to be negative. Are you sure to continue?');
      if (!response) return;
    }

    if (this.quantity < 1) {
      this.toastr.errorToastr('Please enter a valid quantity.');
      return;
    }

    this.shipmentDetail.qty = this.quantity;
    this.shipmentDetail.unitPrice = this.unitPrice;
    this.shipmentDetail.price = this.shipmentDetail.qty * this.shipmentDetail.unitPrice;
    this.shipmentDetail.partId = this.selectedPart.id;
    this.shipmentDetail.partCode = this.selectedPart.code;
    this.shipmentDetail.partDescription = this.selectedPart.description;
    this.shipmentDetail.orderNo = this.orderNo;
    this.shipment.packingSlipDetails.push(this.shipmentDetail);

    this.shipmentDetail = new PackingSlipDetail();
    this.partId = 0;
    this.unitPrice = 0;
    this.quantity = 0;
    this.orderNo = '';
  }

  save() {
    if (this.shipment.packingSlipDetails.length == 0) {
      this.toastr.errorToastr("Please add at least one part to save");
      return;
    }

    this.shipment.shippingDate = DateHelper.formatDate(new Date(this.shippingDate));
    this.httpLoaderService.show();
    this.invoiceService.saveMonthlyInvoice(this.shipment)
        .subscribe(
          () => {
            this.toastr.successToastr('Monthly invoice saved successfully');
            this.httpLoaderService.hide();
            setTimeout(() => {
              this.route.navigateByUrl('companies/monthly-invoice');
            })
          }, 
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoaderService.hide();
          }
        )
  }
}