import { Component } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component'; // Import NavbarComponent
import { FooterComponent } from '../footer/footer.component';
@Component({
  standalone: true,
  selector: 'app-contact',
  imports: [ NavbarComponent, FooterComponent],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.css']
})
export class ContactComponent {}
