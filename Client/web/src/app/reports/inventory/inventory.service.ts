import { Injectable } from '@angular/core';
import { ApiService } from '../../common/services/api.service';
import { Part } from '../../models/part.model';
import { ConfigService } from '../../config/config.service';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {

  constructor(private config: ConfigService, private apiService: ApiService) { }

  getAllParts() {
    return this.apiService.get<Part[]>(this.config.Settings.apiServerHost + this.config.Settings.partsUri);
  }
}
