import { Part } from './part.model';

export class PurchaseOrder {
    id: number = 0;
    companyId: number = 0;
    customerId: number = 0;
    supplierId: number = 0;
    isBlanketPO: boolean = false;
    poNo: string;
    emailIds: string;
    remarks: string;
    paymentTerms: string;
    deliveryTerms: string;
    isClosed: boolean = false;
    isAcknowledged: boolean;
    poDate: string;
    dueDate: string;
    closingDate: string;
    acknowledgementDate: string;
    poDetails: PurchaseOrderDetail[];
    orderDetails: PurchaseOrderDetail[];
    poTerms: PurchaseOrderTerm[];
    supplierName: string;
    customerName: string;

    constructor() {
        this.poDetails = [];
        this.orderDetails = [];
        this.poTerms = [];
    }
}

export class PurchaseOrderDetail {
    id: number;
    poId: number;
    partId: number;
    blanketPOId: number;
    qty: number;
    blanketPOAdjQty: number;
    lineNumber: number;
    unitPrice: number;
    ackQty: number;
    inTransitQty: number;
    receivedQty: string;
    referenceNo: string;
    note: string;
    dueDate: string;
    closingDate: Date;
    isClosed: boolean;
    partCode: string;
    description: string;
    total: string;
    part: Part
    shippedQty: number = 0;
    orderId: number = 0;
    srNo: number = 0;
    openQty: number = 0;
    isAcknowledged: boolean = false;
    isForceClosed: boolean = false;
    packingSlipNo: string = '';
    shippingDate: string;
    invoiceNo: string = '';
    partAcknowledgementDate: string;
}

export class PurchaseOrderTerm {
    id: number = 0;
    poId: number = 0;
    sequenceNo: number = 0;
    term: string = '';
}