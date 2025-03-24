Purpose:
“CardCraze” is an online card shop that allows users to browse, purchase, and manage their collection of trading cards. The platform offers an easy shopping experience with features like user authentication, a wishlist for future purchases, and a personal collection tracker. By integrating a structured shopping cart and user profiles, the system ensures a streamlined and interactive experience for collectors and enthusiasts alike!

Adrian Agius: Shopping and Cart Pages:
Use Case Responsibilities:
-Browsing/Searching Cards
-Browse cards by category (rarity, type, etc)
-Search for specific cards
-View card details
-Shopping Cart
-Add/Remove/Update items in cart
-Checkout process (payment and shipping)
-----------------------------------------
Classes to Implement:
Models
-Cart Class
Controllers
-ShopController
-CartController
Views
-Shop.cshtml
-Cart.cshtml

Max Matthews: Wishlist and Home Pages:
Use Case Responsibilities:
-Wishlist Management
-Add/remove cards from wishlist
-Move cards from wishlist to cart
-Home Page
-Handle navigation between different pages (profile page, wishlist, shopping)
-Display “featured” cards
-Implement intuitive UI
--------------------------
Classes to Implement:
Models
-Card class
-Wishlist class
Controllers
-WishlistController
-HomeController
Views
-Home.cshtml
-Wishlist.cshtml

Tristan Ung: Login and Profile Pages:
Use Case Responsibilities:
-User Authentication
-New users can register for an account
-Registered users can login and access their account
-Users can reset their password if they ever forget
-User Profile and Collection Management
-Users can update their profile details (e.g. shipping address, payment info, etc)
-Users can view their order history
-Users can view and categorize their personal card collection
-------------------------------------------------
Classes to Implement:
Models
-User class
-Collection class
Controllers
-AccountController
-ProfileController
Views
-Login.cshtml
-Register.cshtml
-Profile.cshtml
