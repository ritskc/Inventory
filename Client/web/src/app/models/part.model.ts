
export class Part {

    constructor() {
        this.partSupplierAssignments = [];
        this.partCustomerAssignments = [];
    }

    id: number = 0;
    code: string;
    description: string;
    companyId: number = 0;
    weightInKg: number = 0;
    weightInLb: number = 0;
    openingQty: number = 0;
    minQty: number = 0;
    maxQty: number = 0;
    drawingNo: number = 0;
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
    id: number = 0;
    supplierID: number = 0;
    mapCode: string;
    description: string;
    qtyInHand: number = 0;
    qtyInTransit: number = 0;
    totalQty: number = 0;
    unitPrice: number = 0;
}

export class PartCustomerAssignment
{
    id: number = 0;
    customerId: number = 0;
    mapCode: string;
    description: string;
    weight: number = 0;
    rate: number = 0;
    surchargePerPound: number = 0;
    openingQty: number = 0;
    surchargeExist: boolean;
}