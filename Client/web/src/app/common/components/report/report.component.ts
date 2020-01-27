import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss']
})
export class ReportComponent implements OnInit {

  private safeUrl: SafeResourceUrl = '';
  private display: boolean = false;
  
  @Output() closeEvent: EventEmitter<any> = new EventEmitter();
  @Input() displayReportEvent: Observable<string>;
  
  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.displayReportEvent.subscribe((url: string) => {
      this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
      this.display = true;
    })
  }

  close() {
    this.display = false;
    this.closeEvent.emit(true);
  }
}
