import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { ConfigService } from '../../config/config.service';
import { Observable } from 'rxjs';
import { Part, PartCosting } from '../../models/part.model';

@Injectable({
  providedIn: 'root'
})
export class PartsService {

  constructor(private apiService: ApiService, private configService: ConfigService,) { 
    
  }

  getAllParts(companyId: number): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }`);
  }

  getAllPartsInWarehouse(companyId: number, warehouseId: number): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/warehouse/${ warehouseId }`);
  }

  getInventoryForDateRange(companyId: number, date: any): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/inventory/${ date }`);
  }

  getWarehouseInventoryForDateRange(companyId: number, warehouseId: number, date: any): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.warehousesUri }/${ companyId }/${ warehouseId }/${ date }`);
  }

  getPart(companyId: number, partId: number): Observable<Part> {
    return this.apiService.get<Part>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ partId }`);
  }

  save(part: Part) {
    if (part.id > 0) 
      return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ part.id }`);
    else
      return this.apiService.post<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }`);
  }

  delete(id: number) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }`);
  }

  updateOpeningQuantity(part: Part, companyId: number, partId: number, quantity: number) {
    return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/OpeningQty/${ partId }/${ quantity }`);
  }

  updateOpeningQuantityByPartCode(part: Part, companyId: number, partId: number, quantity: number) {
    return this.apiService.put<Part>(part, `${ this.configService.Settings.apiServerHost }${ this.configService.Settings.partsUri }/${ companyId }/${ partId }/${ quantity }`);
  }

  adjustPart(partId: number, direction: string, notes: string, companyId: number, quantity: number, monthlyCustomer: boolean) {
    var monthlyCustomerText = monthlyCustomer ? 'monthly': 'regular';
    return this.apiService.post<Part>(null, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ partId }/${ monthlyCustomerText }/${ direction }/${ quantity }/${ notes }`);
  }

  getPartsStatus(companyId: number, partId: number, type: string): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/${ type }/${ partId }`);
  }

  updatePartCosting(companyId: number, part: Part) {
    return this.apiService.post<PartCosting[]>(part.stockPrices, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ part.id }`);
  }

  updatePartCostingByPart(companyId: number, partCosting: PartCosting[]) {
    return this.apiService.put<PartCosting[]>(partCosting, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/import/${ companyId }`);
  }

  transferToAnotherWarehouse(transfer: any) {
    return this.apiService.put<Part>(transfer, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }`);
  }

  getWarehouseInventory(companyId: number, partId: number): Observable<any[]> {
    return this.apiService.get<any[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.partsUri }/${ companyId }/WarehouseInventory/${ partId }`);
  }

  getAllAssembliesInCompany(companyId: number): Observable<Part[]> {
    return this.apiService.get<Part[]>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.assemblyUri }/${ companyId }`);
  }

  getAnAssembly(companyId: number, id: number): Observable<Part> {
    return this.apiService.get<Part>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.assemblyUri }/${ companyId }/${ id }`);
  }

  createAssembly(assembly: any) {
    return this.apiService.post<any>(assembly, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.assemblyUri }`);
  }

  deleteAnAssembly(id: number, body: any) {
    return this.apiService.delete(id, `${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.assemblyUri }`, body);
  }

  getStockPrice(companyId: number): Observable<any> {
    return this.apiService.get<any>(`${ this.configService.Settings.apiServerHost }/reports/stockprice/${ companyId }`);
  }
}