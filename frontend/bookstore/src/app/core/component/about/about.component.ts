import { Component } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component'; // Import NavbarComponent
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-about',
  imports: [NavbarComponent, ],
  templateUrl: './about.component.html',
  styleUrl: './about.component.css'
})
export class AboutComponent {

}
