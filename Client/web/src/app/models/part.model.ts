
export class Part {

    constructor() {
        this.partSupplierAssignments = [];
        this.partCustomerAssignments = [];
    }

    id: number;
    code: string;
    description: string;
    companyId: number;
    weightInKg: number;
    weightInLb: number;
    openingQty: number;
    minQty: number;
    maxQty: number;
    drawingNo: number;
    drawingUploaded: boolean;
    drawingFileName: string;
    location: string;
    isActive: boolean;
    isSample: boolean;
    
    partSupplierAssignments: PartSupplierAssignment[];
    partCustomerAssignments: PartCustomerAssignment[];
}

export class PartSupplierAssignment
{
    id: number;
    supplierID: number;
    mapCode: string;
    description: string;
    qtyInHand: number;
    qtyInTransit: number;
    totalQty: number;
    unitPrice: number;
}

export class PartCustomerAssignment
{
    id: number;
    customerId: number;
    mapCode: string;
    description: string;
    weight: number;
    rate: number;
    surchargePerPound: number;
    openingQty: number;
    surchargeExist: boolean;
}