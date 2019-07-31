import { Component, OnInit, Input } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss']
})
export class ReportComponent implements OnInit {

  private safeUrl: SafeResourceUrl = '';
  
  @Input() display: boolean = false;

  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.reportSourceUrl);
  }

  @Input() reportSourceUrl: string = 'http://renovate.yellow-chips.com/ReportViewer/invoice.aspx?id=1008';

  close() {
    this.display = false;
  }
}
