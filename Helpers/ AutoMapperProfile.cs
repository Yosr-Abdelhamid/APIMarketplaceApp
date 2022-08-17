using APIMarketplaceApp.Models;
using AutoMapper;

namespace APIMarketplaceApp.Helpers
{
    public class  AutoMapperProfile  : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, Vendeur>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

            CreateMap<CommandeRequest, Commande>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

             CreateMap<RequestPortfeuille, PortfeuilleVendeur>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));   

            CreateMap<ClientRequest, Client>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));
                 CreateMap<UserAdminRequest, Admin>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                CreateMap<RequestProduct, ProductVend>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                CreateMap<ProductModel, ProductVend>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                CreateMap<ImageUpload, ProductVend>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));


                 CreateMap<ImageOrganisation, Vendeur>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                CreateMap<UpdateVendeur, Vendeur>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));
                
                 CreateMap<UpdateClient, Client>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                 CreateMap<RequestOrder, Commande>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

                  CreateMap<PayedRequest, OrderBySellerPayed>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                        if (x.DestinationMember.Name == "Role") return false;

                        return true;
                    }
                ));

        }
    }
  
    }
