import { Invoice } from "./invoice.model";

export class Container {
    id: number = 0;
    companyId: number;
    containerNo: string;
    eta: string;
    invoices: string;
    invoiceDate: string;
    isAirShipment: boolean;
    byCourier: boolean;
    isContainerReceived: boolean;
    receivedDate: string;
    supplierInvoices: Invoice[] = [];
}