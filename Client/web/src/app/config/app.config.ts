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
    shipmentUri: string;
    masterShipmentUri: string;
    barcodeUri: string;
    customerInvoiceUri: string;
    fileApiUri: string;
    reportsUri: string;
    entityTracker: string;
    directSupplierPo: string;

    initialize() {
        this.apiServerHost = 'https://questapi.yellow-chips.com';
        this.reportsUri = 'https://renovate.yellow-chips.com/ReportViewer/';
        // this.apiServerHost = 'http://po.harisons.com/api';
        // this.reportsUri = 'http://po.harisons.com/reports/ReportViewer/';

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
        this.shipmentUri = '/PackingSlips';
        this.masterShipmentUri = '/MasterPackingSlips';
        this.customerInvoiceUri =  '/Invoices';
        this.entityTracker = '/EntityTracker';
        this.directSupplierPo = '/SupplierAccess';
        this.barcodeUri = 'https://quest.yellow-chips.com/static/barcode.html?';
    }
}

export class GridConstants {
    public readonly Company = 1;
}