import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';
import { CartService } from '../../../../services/cart.service';
import { ProductService } from '../../../../services/product.service';
import { AuthService } from '../../../../services/auth.service'; // Import AuthService

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent, FooterComponent],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css'],
})
export class CartComponent implements OnInit {
  cartItems: any[] = []; // Lưu danh sách sản phẩm trong giỏ hàng
  errorMessage: string = '';
  isLoading = true; // Trạng thái tải dữ liệu

  constructor(
    private cartService: CartService,
    private productService: ProductService,
    private authService: AuthService, // Inject AuthService
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCartItems();
  }

  // Load giỏ hàng từ API
  loadCartItems(): void {
    const userId = this.authService.getCurrentUserId(); // Lấy userId từ AuthService
    if (!userId) {
      console.error('Không tìm thấy User ID.');
      this.errorMessage = 'Vui lòng đăng nhập để xem giỏ hàng.';
      this.isLoading = false;
      return;
    }

    this.cartService.getCartByUserId(userId).subscribe({
      next: (response) => {
        if (response && Array.isArray(response.data)) {
          this.cartItems = response.data;
          this.fetchProductDetails(); // Lấy thông tin sản phẩm từ ProductService
        } else {
          console.error('Dữ liệu trả về không phải là mảng:', response);
          this.errorMessage = 'Không thể tải giỏ hàng. Dữ liệu không hợp lệ.';
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

  // Lấy thông tin sản phẩm cho từng mục trong giỏ hàng
  fetchProductDetails(): void {
    if (!Array.isArray(this.cartItems) || this.cartItems.length === 0) {
      console.warn('Giỏ hàng rỗng hoặc không hợp lệ.');
      this.isLoading = false;
      return;
    }

    const productRequests = this.cartItems.map((item) =>
      this.productService.getProductById(item.bookId).toPromise()
    );

    Promise.all(productRequests)
      .then((products) => {
        this.cartItems = this.cartItems.map((cartItem, index) => ({
          ...cartItem,book: products[index],
        }));
        this.isLoading = false;
      })
      .catch((err) => {
        console.error('Không thể lấy thông tin sản phẩm:', err);
        this.errorMessage = 'Không thể lấy thông tin sản phẩm. Vui lòng thử lại sau!';
        this.isLoading = false;
      });
  }

  // Cập nhật số lượng giỏ hàng
  updateCart(item: any): void {
    const updatedCart = {
      userId: item.userId,
      bookId: item.bookId,
      quantity: item.quantity,
    };

    this.cartService.updateCart(item.userId, item.bookId, updatedCart).subscribe({
      next: () => {
        console.log('Cập nhật giỏ hàng thành công!');
      },
      error: (err) => {
        console.error('Cập nhật giỏ hàng thất bại:', err);
        this.errorMessage = 'Cập nhật giỏ hàng thất bại. Vui lòng thử lại sau!';
      },
    });
  }

  // Xóa sản phẩm khỏi giỏ hàng
  removeFromCart(item: any): void {
    this.cartService.deleteCart(item.userId, item.bookId).subscribe({
      next: () => {
        console.log('Xóa sản phẩm khỏi giỏ hàng thành công!');
        this.loadCartItems();
      },
      error: (err) => {
        console.error('Xóa sản phẩm khỏi giỏ hàng thất bại:', err);
        this.errorMessage = 'Xóa sản phẩm khỏi giỏ hàng thất bại. Vui lòng thử lại sau!';
      },
    });
  }

  // Điều hướng đến trang thanh toán
  proceedToCheckout(): void {
    this.router.navigate(['/checkout']);
  }

  // Điều hướng về trang chủ
  proceedToHome(): void {
    this.router.navigate(['/']);
  }
}