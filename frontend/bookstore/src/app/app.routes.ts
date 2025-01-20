import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ContactComponent } from './core/component/contact/contact.component';
import { ProductDetailComponent } from './features/home/books/product-detail/product-detail.component';
import { LoginComponent } from './features/home/account/login/login.component';
import { CartComponent } from './features/home/books/cart/cart.component';
import { CheckoutComponent } from './features/home/books/checkout/checkout.component';
import { CategoryComponent } from './features/home/books/category/category.component';
import { SearchComponent } from './features/home/books/search/search.component';
import { AboutComponent } from './core/component/about/about.component';
import { NavbarComponent } from './core/component/navbar/navbar.component';
import { FooterComponent } from './core/component/footer/footer.component';
import { RegisterComponent } from './features/home/account/register/register.component';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent }, // Trang mặc định
  { path: 'contact', component: ContactComponent }, // Trang liên hệ
  { path: 'product-detail/:id', component: ProductDetailComponent } ,
  { path: 'login', component: LoginComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'search', component: SearchComponent },
  { path: 'category', component: CategoryComponent },
  { path: 'about', component: AboutComponent },
  { path: 'navbar', component: NavbarComponent },
  { path: 'footer', component: FooterComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'cart', component: CartComponent }
];
export const routes: Routes = [
  { path: '', component: HomeComponent }, // Trang mặc định
  { path: 'contact', component: ContactComponent }, // Trang liên hệ
  { path: 'product-detail/:id', component: ProductDetailComponent } ,
  { path: 'login', component: LoginComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'search', component: SearchComponent },
  { path: 'category', component: CategoryComponent },
  { path: 'about', component: AboutComponent },
  { path: 'navbar', component: NavbarComponent },
  { path: 'footer', component: FooterComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'cart', component: CartComponent }

];
