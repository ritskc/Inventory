import { Component, OnInit, Input } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss']
})
export class ReportComponent implements OnInit {

  private safeUrl: SafeResourceUrl = '';
  private display: boolean = false;
  
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
  }
}
