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
  selectedShipment: Shipment;
  shipments: Shipment[] = [];
  filteredShipments: any[] = [];
  customers: Customer[] = [];
  columns: DataColumn[] = [];
  shipmentBoxesGridColumns: DataColumn[] = [];
  customerId: number = -1;
  showFullDetails: boolean = false;
  showInvoiced: boolean = false;
  showRepackge: boolean = false;
  printDocument: Subject<string> = new Subject<string>();
  showModal: boolean = false;
  boxes: number = 0;
  packingSlipDetailId: number = 0;
  shipmentBoxes: any[] = [];
  isBarcodeMode: boolean = false;

  constructor(private companyService: CompanyService, private shipmentService: ShipmentService, private customerService: CustomerService, 
    private router: Router, private httpLoader: httpLoaderService, private toastr: ToastrManager) { }

  ngOnInit() {
    if (window.location.hash.indexOf('/barcode') > 0)
      this.isBarcodeMode = true;

    this.currentlyLoggedInCompany = this.companyService.getCurrentlyLoggedInCompanyId();
    this.initializeGridColumns();
    this.loadAllCustomers();
  }

  initializeGridColumns() {
    this.columns = [];
    this.columns.push( new DataColumn({ headerText: "Customer", value: "customerName", sortable: true, customStyling: 'column-width-150' }) );
    this.columns.push( new DataColumn({ headerText: "Slip No", value: "packingSlipNo", sortable: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Date", value: "shippingDate", sortable: true, isDate: true }) );
    this.columns.push( new DataColumn({ headerText: "Shipped Via", value: "shipVia", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Gross Weight", value: "grossWeight", sortable: false, customStyling: 'right' }) );
    this.columns.push( new DataColumn({ headerText: "Tracking", value: "trakingNumber", sortable: false }) );
    this.columns.push( new DataColumn({ headerText: "Invoice", value: "isInvoiceCreated", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "Payment", value: "isPaymentReceived", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    this.columns.push( new DataColumn({ headerText: "POS", value: "isPOSUploaded", sortable: false, isBoolean: true, customStyling: 'center', isDisabled: true }) );
    
    // this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
    //   new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editShipment' }),
    //   new DataColumnAction({ actionText: 'Invoice', actionStyle: ClassConstants.Primary, event: 'editInvoice' }),
    //   new DataColumnAction({ actionText: 'Repack', actionStyle: ClassConstants.Primary, event: 'printRepackingInvoice', showOnlyIf: 'data["isRepackage"] == true' }),
    //   new DataColumnAction({ actionText: 'Verify', actionStyle: ClassConstants.Primary, event: 'verifyShipment', showOnlyIf: 'data["isShipmentVerified"] == false' }),
    //   new DataColumnAction({ actionText: 'Allow Scanning', actionStyle: ClassConstants.Primary, event: 'allowScanning', showOnlyIf: 'data["isShipmentVerified"] == true && data["allowScanning"] == false' }),
    //   new DataColumnAction({ actionText: 'Print Shipment', actionStyle: ClassConstants.Primary, event: 'printShipment', showOnlyIf: 'data["isShipmentVerified"] == true' }),
    //   new DataColumnAction({ actionText: 'Print Invoice', actionStyle: ClassConstants.Primary, event: 'printInvoice' }),
    //   new DataColumnAction({ actionText: 'Barcode', actionStyle: ClassConstants.Primary, event: 'printBarcode', showOnlyIf: 'data["isShipmentVerified"] == true' }),
    //   new DataColumnAction({ actionText: 'Print BL', actionStyle: ClassConstants.Primary, event: 'printBL', showOnlyIf: 'data["isScanned"] == true' }),
    //   new DataColumnAction({ actionText: 'Download POS', actionStyle: ClassConstants.Primary, event: 'downloadPOS' }),
    //   new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'delete' })
    // ] }) );
    
    if (this.isBarcodeMode) {
      this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: 'Verify', actionStyle: ClassConstants.Primary, event: 'verifyShipment', showOnlyIf: 'data["isShipmentVerified"] == false' }),
        new DataColumnAction({ actionText: 'Undo Verify', actionStyle: ClassConstants.Primary, event: 'undoVerifyShipment', showOnlyIf: 'data["isShipmentVerified"] == true' }),
        new DataColumnAction({ actionText: 'Auto Scanning', actionStyle: ClassConstants.Primary, event: 'autoScanning' }),
        new DataColumnAction({ actionText: 'Allow Scanning', actionStyle: ClassConstants.Primary, event: 'allowScanning', showOnlyIf: 'data["isShipmentVerified"] == true && data["allowScanning"] == false' }),
        new DataColumnAction({ actionText: 'Barcode', actionStyle: ClassConstants.Primary, event: 'printBarcode', showOnlyIf: 'data["isShipmentVerified"] == true' }),
        new DataColumnAction({ actionText: 'Scan Barcode', actionStyle: ClassConstants.Primary, event: 'scanBarcode', showOnlyIf: 'data["isShipmentVerified"] == true' })
      ] }) );
    } else {
      this.columns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
        new DataColumnAction({ actionText: 'Update', actionStyle: ClassConstants.Warning, event: 'editShipment' }),
        new DataColumnAction({ actionText: 'Invoice', actionStyle: ClassConstants.Primary, event: 'editInvoice' }),
        new DataColumnAction({ actionText: 'Repack', actionStyle: ClassConstants.Primary, event: 'printRepackingInvoice', showOnlyIf: 'data["isRepackage"] == true' }),
        new DataColumnAction({ actionText: 'Print Shipment', actionStyle: ClassConstants.Primary, event: 'printShipment', showOnlyIf: 'data["isShipmentVerified"] == true' }),
        new DataColumnAction({ actionText: 'Print Invoice', actionStyle: ClassConstants.Primary, event: 'printInvoice' }),
        new DataColumnAction({ actionText: 'Print BL', actionStyle: ClassConstants.Primary, event: 'printBL', showOnlyIf: 'data["isScanned"] == true' }),
        new DataColumnAction({ actionText: 'Download POS', actionStyle: ClassConstants.Primary, event: 'downloadPOS' }),
        new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'delete' })
      ] }) );
    }
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
    this.columns.push( new DataColumn({ headerText: "Gross Weight", value: "grossWeight", sortable: false, customStyling: 'right' }) );
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
          if (this.isBarcodeMode)
            this.filteredShipments = this.filteredShipments.filter(s => s.isScanned == false);
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
      case 'verifyShipment':
        this.verifyShipment(data);
        break;
      case 'undoVerifyShipment':
        this.undoVerifyShipment(data);
        break;
      case 'allowScanning':
        this.allowScanning(data);
        break;
      case 'autoScanning':
        this.autoScanning(data);
        break;
      case 'printBarcode':
        this.printBarcode(data);
        break;
      case 'scanBarcode':
        this.scanBarcode(data);
        break;
    }
  }

  actionButtonClickedForQuantityGrid(data) {
    switch(data.eventName) {
      case 'delete':
        this.removeShipmentBoxAllocation(data);
        break;
    }
  }

  removeShipmentBoxAllocation(data) {
    var index = this.shipmentBoxes.findIndex(d => d == data);
    this.shipmentBoxes.splice(index, 1);
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
          viewModel.grossWeight = shipment.grossWeight;
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

  partSelected() {
    var selctedPart = this.selectedShipment.packingSlipDetails.find(d => d.id == this.packingSlipDetailId);
    this.boxes = selctedPart.boxes;
  }

  addBoxesToShipment() {
    this.shipmentBoxesGridColumns = [];
    this.shipmentBoxesGridColumns.push(new DataColumn({ headerText: "Part", value: "description" }));
    this.shipmentBoxesGridColumns.push(new DataColumn({ headerText: "Box", value: "boxeNo" }));
    this.shipmentBoxesGridColumns.push(new DataColumn({ headerText: "Qty", value: "qty", isEditable: true }));
    this.shipmentBoxesGridColumns.push( new DataColumn({ headerText: "Action", value: "Action", isActionColumn: true, customStyling: 'center', actions: [
      new DataColumnAction({ actionText: 'Delete', actionStyle: ClassConstants.Danger, event: 'delete' })
    ] }) );

    var selctedPart = this.selectedShipment.packingSlipDetails.find(d => d.id == this.packingSlipDetailId);

    if (this.shipmentBoxes.filter(s => s.packingSlipDetailId == this.packingSlipDetailId).length > 0) {
      this.toastr.errorToastr('This line item has been already added.');
      return;
    }

    if (selctedPart.qty < this.boxes) {
      this.toastr.errorToastr('Number of boxes cannot be more than quantity')
      return;
    }

    if (this.boxes == 0) {
      this.toastr.errorToastr('Number of boxes should be more than zero');
      return;
    }

    var remainingQuantities = selctedPart.qty;
    for (var boxIndex = 1; boxIndex <= this.boxes; boxIndex++) {
      var box = {
        id: 0,
        description: selctedPart.partDetail.description,
        packingSlipId: this.selectedShipment.id,
        packingSlipDetailId: selctedPart.id,
        partId: selctedPart.partId,
        qty: boxIndex == this.boxes ? remainingQuantities: Math.floor(selctedPart.qty / this.boxes),
        boxeNo: boxIndex,
        barcode: '',
        isScanned: false
      };
      remainingQuantities = remainingQuantities - box.qty;
      this.shipmentBoxes.push(box);
    }
  }

  verifyShipment(data) {
    this.showModal = true;
    this.selectedShipment = data;
    this.boxes = 0;
    this.shipmentBoxes = [];
  }

  undoVerifyShipment(data) {
    if (!confirm('Are you sure you want to undo this operation?')) {
      return;
    }

    this.httpLoader.show();
    this.shipmentService.undoVerifyShipment(data)
        .subscribe(
          () => {
            this.toastr.successToastr('Shipment verification undone successfully!!');
            this.httpLoader.hide();
            this.loadAllCustomers();
            this.showModal = false;
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoader.hide();
          }
        );
  }

  verifyShipmentSave() {
    var hasError = false;
    this.selectedShipment.packingSlipDetails.forEach(detail => {
      var boxes = this.shipmentBoxes.filter(s => s.packingSlipDetailId == detail.id);
      var totalQuantitiesInAllBoxes = boxes.reduce((a, b) => a + b.qty, 0);
      if (totalQuantitiesInAllBoxes !== detail.qty) {
        hasError = true;
        this.toastr.errorToastr(`Total quantities in all boxes does not match for ${ detail.partDetail.description }`);
        return;
      }
      detail.boxes = boxes.length;
      detail.packingSlipBoxDetails = [];
      detail.packingSlipBoxDetails.push(...boxes);
    });

    this.selectedShipment.packingSlipDetails.forEach(detail => {
      if (detail.packingSlipBoxDetails.length == 0) {
        hasError = true;
        this.toastr.errorToastr('Could not verify for all products. Please enter box details for all line items');
        return;
      }
    });

    if (hasError) {
      return;
    }

    this.httpLoader.show();
    this.shipmentService.verifyShipment(this.selectedShipment)
        .subscribe(
          () => {
            this.toastr.successToastr('Shipment verified successfully!!');
            this.httpLoader.hide();
            this.loadAllCustomers();
            this.showModal = false;
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoader.hide();
          }
        );
  }

  allowScanning(data) {
    this.httpLoader.show();
    this.shipmentService.allowScanning(data)
        .subscribe(
          () => {
            this.toastr.successToastr('Scanning enabled for the selected shipment!!');
            this.httpLoader.hide();
            this.loadAllCustomers();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoader.hide();
          }
        );
  }

  autoScanning(data) {
    this.httpLoader.show();
    this.shipmentService.autoScanning(data)
        .subscribe(
          () => {
            this.toastr.successToastr('Auto scanning done successfully!!');
            this.httpLoader.hide();
            this.loadAllCustomers();
          },
          (error) => {
            this.toastr.errorToastr(error.error);
            this.httpLoader.hide();
          }
        );
  }

  printBarcode(data) {
    try{
      var appConfiguration = new AppConfigurations();
      var boxNos = '';
      data.packingSlipDetails.forEach(detail => {
        detail.packingSlipBoxDetails.forEach(box => {
          boxNos += box.barcode + '|';
        });
      });
      window.open(appConfiguration.barcodeUri + boxNos);
    } catch {
      this.toastr.errorToastr(`Barcode details unavailable for packing slip ${ data.packingSlipNo }`);
    }
  }

  scanBarcode(data) {
    this.router.navigateByUrl(`/barcode/scan/${ data.id }?type=shipment`);
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
  grossWeight: number;
}