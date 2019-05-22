export class PurchaseOrder {
    id: number;
    companyId: number;
    supplierId: number;
    poNo: string;
    emailIds: string;
    remarks: string;
    paymentTerms: string;
    deliveryTerms: string;
    isClosed: boolean;
    isAcknowledged: boolean;
    poDate: Date;
    closingDate: Date;
    acknowledgementDate: Date;
    poDetails: PurchaseOrder[];

    constructor() {
        this.poDetails = [];
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
    dueDate: Date;
    closingDate: Date;
    isClosed: boolean;
}