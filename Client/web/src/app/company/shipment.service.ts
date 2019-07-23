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

  getAllShipments(): Observable<Shipment[]> {
    return this.apiService.get<Shipment[]>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
  }

  getAShipment(shipmentId: number): Observable<Shipment> {
    return this.apiService.get<Shipment>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + '/' + shipmentId);
  }

  createShipment(shipment: Shipment) {
    return this.apiService.post(shipment, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
  }

  deleteShipment(shipmentId: number) {
    return this.apiService.delete(shipmentId, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
  }
}
