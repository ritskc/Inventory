import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { Shipment } from '../models/shipment.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShipmentService {

  constructor(private apiService: ApiService, private config: ConfigService) { }

  getAllShipments(companyId: number): Observable<Shipment[]> {
    return this.apiService.get<Shipment[]>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + '/' + companyId);
  }

  getAShipment(companyId: number, shipmentId: number): Observable<Shipment> {
    return this.apiService.get<Shipment>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + '/1/' + shipmentId);
  }

  createShipment(shipment: Shipment) {
    if (shipment.id < 1) 
      return this.apiService.post(shipment, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
    else
      return this.apiService.put(shipment, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + `/${ shipment.id }`);
  }

  deleteShipment(shipmentId: number) {
    return this.apiService.delete(shipmentId, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
  }

  getLatestShipment(companyId: number, date: string): Observable<any> {
    return this.apiService.get(`${ this.config.Settings.apiServerHost }/${ this.config.Settings.entityTracker }/packing_slip/${ companyId }/${ date }`);
  }
}
