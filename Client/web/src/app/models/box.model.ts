import { Product } from "./product.model";

export class Box {
    id: number = 0;
    companyId: number = 0;
    name: string = '';
    description: string = '';
    barcode: string = '';
    wareHouseId: number = 0;
    locationId: number = 0;
    locationName: string = '';
    products: Product[] = [];
}