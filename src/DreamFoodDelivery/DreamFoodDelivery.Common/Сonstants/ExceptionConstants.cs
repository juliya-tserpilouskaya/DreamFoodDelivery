using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class ExceptionConstants
    {
        public const string SOURCE_IS_NULL = "Source is null. ";
        public const string CANNOT_SAVE_MODEL = "Cannot save model. ";
        public const string CANNOT_SAVE_CHANGES = "Cannot save changes. ";
        public const string CANNOT_UPDATE_MODEL = "Cannot update model. ";
        public const string UNABLE_TO_RETRIEVE_DATA = "Unable to retrieve data. ";
        public const string CLAIM_PRINCIPAL_ERROR = "Claim principal error. ";
        public const string INVALID_REFRESH_TOKEN = "Invalid refresh token. ";

        public const string IDENTITY_USER_WAS_NOT_FOUND = "Identity user was not found. ";
        public const string UNABLE_TO_CREATE_ADMIN = "Unable to create admin.";
        public const string USER_ALREADY_EXIST = "User already exist."; 
        public const string USER_DOES_NOT_EXISTS = "User doesn't exists."; 
        public const string WRONG_PASSWORD = "Wrong password."; 
        public const string CANNOT_CHANGE_ROLE = "Cannot change role."; 

        public const string USER_WAS_NOT_FOUND = "User was not found. ";
        public const string USERS_WERE_NOT_FOUND = "Users were not found. ";
        public const string CANNOT_DELETE_USER = "Cannot delete user. ";

        public const string DISH_WAS_NOT_FOUND = "Dish was not found. ";
        public const string CANNOT_DELETE_DISH = "Cannot delete dish. ";
        public const string DISHES_WERE_NOT_FOUND = "Dishes were not found. ";
        public const string CANNOT_DELETE_DISHES = "Cannot delete dishes. ";

        public const string ORDER_WAS_NOT_FOUND = "Order was not found. ";
        public const string CANNOT_DELETE_ORDER = "Cannot delete order. ";
        public const string ORDERS_WERE_NOT_FOUND = "Orders were not found. ";
        public const string CANNOT_DELETE_ORDERS = "Cannot delete orders. ";

        public const string ORDER_STATUSES_WERE_NOT_FOUND = "Order statuses were not found. ";

        public const string TAGS_WERE_NOT_FOUND = "Tags were not found. ";
        public const string TAG_WAS_NOT_FOUND = "Tag was not found. ";
        public const string CANNOT_DELETE_TAGS = "Cannot delete tags. ";
        public const string CANNOT_DELETE_TAG = "Cannot delete tag. ";

        public const string REVIEW_WAS_NOT_FOUND = "Review was not found. ";
        public const string CANNOT_DELETE_REVIEW = "Cannot delete review. ";
        public const string REVIEWS_WERE_NOT_FOUND = "Reviews were not found. ";
        public const string CANNOT_DELETE_REVIEWS = "Cannot delete review. ";
        public const string EMPTY_RATING = "Check the information, there may be no reviews. ";

        public const string NO_IMAGES = "This dish doesn't have images!";
        public const string IMAGE_IS_NOT_EXISTS = "Image is not exists!";

        public const string BASKET_WAS_NOT_FOUND = "Basket was not found. ";
        public const string DISHES_IN_BASKET_WAS_NOT_FOUND = "Dishes in basket was not found. ";
    }
}
