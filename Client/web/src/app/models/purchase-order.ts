export class PurchaseOrder {
    id: number = 0;
    companyId: number = 0;
    supplierId: number = 0;
    poNo: string;
    emailIds: string;
    remarks: string;
    paymentTerms: string;
    deliveryTerms: string;
    isClosed: boolean = false;
    isAcknowledged: boolean;
    poDate: string;
    closingDate: string;
    acknowledgementDate: string;
    poDetails: PurchaseOrderDetail[];
    poTerms: PurchaseOrderTerm[];

    constructor() {
        this.poDetails = [];
        this.poTerms = [];
    }
}

export class PurchaseOrderDetail {
    id: number;
    poId: number;
    partId: number;
    qty: number;
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
    total: number;
}

export class PurchaseOrderTerm {
    id: number = 0;
    poId: number = 0;
    sequenceNo: number = 0;
    term: string = '';
}