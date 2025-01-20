import { Component, OnInit } from '@angular/core'; 
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { CartService } from '../../../../services/cart.service'; 
import { ProductService } from '../../../../services/product.service'; 
import { AuthService } from '../../../../services/auth.service'; // Import AuthService

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css'],
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  cartItems: any[] = []; // Dữ liệu giỏ hàng từ API
  paymentMethod = 'cod'; // Phương thức thanh toán mặc định
  isLoading = true; // Trạng thái tải
  errorMessage = ''; // Thông báo lỗi

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private cartService: CartService,
    private productService: ProductService,
    private authService: AuthService // Inject AuthService
  ) {
    this.checkoutForm = this.fb.group({
      fullName: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      commune: ['', Validators.required],
      province: ['', Validators.required],
      city: ['', Validators.required],
      streetAddress: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadCartItems(); // Tải dữ liệu giỏ hàng
  }

  // Tải dữ liệu giỏ hàng từ API và thêm thông tin giá từ ProductService
  loadCartItems(): void {
    const userId = this.authService.getCurrentUserId(); // Lấy userId từ AuthService
    if (!userId) {
      console.error('Không tìm thấy User ID. Vui lòng đăng nhập lại.');
      this.errorMessage = 'Bạn cần đăng nhập để xem giỏ hàng.';
      this.isLoading = false;
      return;
    }

    this.cartService.getCartByUserId(userId).subscribe({
      next: (response) => {
        if (response && response.data) {
          const cartItems = response.data; // Giả sử API trả về { data: [...] }

          // Lấy thông tin giá từ ProductService cho từng sản phẩm
          const productRequests = cartItems.map((item: any) =>
            this.productService.getProductById(item.bookId).toPromise()
          );

          Promise.all(productRequests)
            .then((products) => {
              // Kết hợp dữ liệu sản phẩm với giỏ hàng
              this.cartItems = cartItems.map((cartItem: any, index: number) => ({
                ...cartItem,
                price: products[index]?.price || 0, // Lấy giá từ API sản phẩm
                image: products[index]?.imageUrl || '', // Lấy hình ảnh từ API sản phẩm
              }));
              console.log('Cart items with product details:', this.cartItems);
              this.isLoading = false;
            })
            .catch((err) => {
              console.error('Không thể lấy thông tin sản phẩm:', err);
              this.errorMessage = 'Không thể lấy thông tin sản phẩm. Vui lòng thử lại sau!';
              this.isLoading = false;
            });
        } else {
          this.errorMessage = 'Không tìm thấy sản phẩm trong giỏ hàng.';
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('Không thể tải giỏ hàng:', err);
        this.errorMessage = 'Không thể tải giỏ hàng. Vui lòng thử lại sau!';
        this.isLoading = false;
      },
    });
  }

  proceedToCart(): void {
    this.router.navigate(['/cart']);
  }

  get subtotal(): number {
    return this.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  get totalQuantity(): number {
    return this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  onSubmit(): void {
    if (this.checkoutForm.valid) {
      alert('Đặt hàng thành công!');
      console.log('Order Details:', this.checkoutForm.value, 'Payment:', this.paymentMethod);
    } else {
      alert('Vui lòng điền đầy đủ thông tin.');
    }
  }
}
