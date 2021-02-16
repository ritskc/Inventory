import { environment } from '../../environments/environment';

export class AppConfigurations {

    constructor() {
        this.initialize();
    }

    apiServerHost: string;
    partsUri: string;
    companyUri: string;
    usersUri: string;
    gridDefinitionUri: string;
    supplierUri: string;
    customerUri: string;
    posUri: string;
    ordersUri: string;
    invoiceUri: string;
    monthlyInvoiceUri: string;
    shipmentUri: string;
    masterShipmentUri: string;
    barcodeUri: string;
    customerInvoiceUri: string;
    fileApiUri: string;
    reportsUri: string;
    entityTracker: string;
    directSupplierPo: string;
    privilegesUri: string;
    userReportsUri: string;
    warehousesUri: string;

    initialize() {
        if (environment.production) {
            this.apiServerHost = 'https://erp.harisons.com/api';
            this.reportsUri = 'https://erp.harisons.com/reports/ReportViewer/';
        } else {
            this.apiServerHost = 'http://cf.globalexportech.com/api';
            this.reportsUri = 'http://cf.globalexportech.com/reports/ReportViewer';
        }

        this.fileApiUri = `${this.apiServerHost}/File`;

        this.usersUri = '/users';
        this.partsUri = '/parts';
        this.companyUri = '/companies';
        this.gridDefinitionUri = '/users';
        this.supplierUri = '/suppliers'
        this.customerUri = '/customers'
        this.posUri = '/pos';
        this.ordersUri = '/orders';
        this.invoiceUri = '/supplierinvoice';
        this.monthlyInvoiceUri = '/monthlyInvoices';
        this.shipmentUri = '/PackingSlips';
        this.masterShipmentUri = '/MasterPackingSlips';
        this.customerInvoiceUri =  '/Invoices';
        this.entityTracker = '/EntityTracker';
        this.directSupplierPo = '/SupplierAccess';
        this.privilegesUri = '/priviledges';
        this.userReportsUri = '/UserReports';
        this.warehousesUri = 'Warehouses';
        this.barcodeUri = 'https://quest.yellow-chips.com/static/barcode.html?';
    }
}

export class GridConstants {
    public readonly Company = 1;
}