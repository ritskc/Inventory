import { Shipment } from './shipment.model';

export class MasterShipment {
    id: number = 0;
    companyId: number = 0;
    customerId: number = 0;
    customerName: string = '';
    masterPackingSlipNo: string = '';
    packingSlipNumbers: string = '';
    comment: string = '';
    updatedDate: string = '';
    isPOSUploaded: boolean = false;
    packingSlips: Shipment[] = [];
}