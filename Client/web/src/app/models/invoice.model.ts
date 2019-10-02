import { Company } from './company.model';
import { Supplier } from './supplier.model';
import { Part } from './part.model';

export class Invoice {

    constructor() {
        this.companyDetail = new Company();
        this.supplierDetail = new Supplier();
        this.supplierInvoiceDetails = [];
    }

    id: number = 0;
    companyId: number = 0;
    companyName: string = '';
    supplierId: number = 0;
    supplierName: string = '';
    invoiceNo: string = '';
    invoiceDate: string = '';
    eta: string = '';
    isAirShipment: boolean = false;
    poNo: string = '';
    referenceNo: string = '';
    email: string = '';
    byCourier: boolean = false;
    isInvoiceUploaded: boolean = false;
    isPackingSlipUploaded: boolean = false;
    isTenPlusUploaded: boolean = false;
    isBLUploaded: boolean = false;
    isTCUploaded: boolean = false;
    invoicePath: string = '';
    packingSlipPath: string = '';
    tenPlusPath: string = '';
    blPath: string = '';
    isInvoiceReceived: boolean = false;
    uploadedDate: string = '';
    receivedDate: string = '';
    companyDetail: Company;
    supplierDetail: Supplier;
    supplierInvoiceDetails: InvoiceDetail[];
}

export class UploadInvoice {

    constructor() {
        this.supplierInvoiceDetails = [];
    }

    InvoiceNo: string = '';
    InvoiceDate: string = '';
    SupplierName: string = '';
    PoNo: string = '';
    CompanyName: string = '';
    ETA: string = '';
    UploadedDate: string = '';
    InvoiceTotal: number = 0;
    supplierInvoiceDetails: UploadInvoiceDetail[];
}

export class UploadInvoiceDetail {
    PartCode: string = '';
    Qty: number = 0;
    Price: number = 0;
    Total: number = 0;
    BoxNumber: string = '';
}

export class InvoiceDetail {

    constructor() {
        this.partDetail = new Part();
    }

    id: number = 0;
    invoiceId: string = '';
    srNo: number = 0;
    partId: number = 0;
    partCode: string = '';
    qty: number = 0;
    price: number = 0;
    total: number = 0;
    adjustedPOQty: number = 0;
    excessQty: number = 0;
    boxNo: number = 0;
    partDetail: Part;
    barcode: string = '';
}