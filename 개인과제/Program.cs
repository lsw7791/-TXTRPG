using System;
using System.Collections.Generic;
using System.IO;

namespace 개인과제
{
    class Dungeon
    {
        public string Name { get; set; }
        public int RecommendedDefense { get; set; }
        public int BaseReward { get; set; }

        public Dungeon(string name, int recommendedDefense, int baseReward)
        {
            Name = name;
            RecommendedDefense = recommendedDefense;
            BaseReward = baseReward;
        }

        // 던전 수행
        public void Enter(Player player)
        {
            Console.WriteLine($"\n[{Name} 던전]에 입장했습니다.");
            int playerDefense = player.BaseDefensePower + player.GetTotalEquippedDefenseBonus();

            // 방어력 비교 후 결과 처리
            if (playerDefense < RecommendedDefense)
            {
                // 40% 확률로 실패
                if (new Random().Next(100) < 40)
                {
                    player.Health /= 2;
                    Console.WriteLine($"방어력이 부족하여 던전 실패! 체력이 절반 감소하여 {player.Health}이(가) 되었습니다.");
                    return;
                }
            }

            // 던전 클리어 성공 시 체력 감소 계산
            int healthReduction = CalculateHealthReduction(playerDefense);
            player.Health -= healthReduction;
            Console.WriteLine($"던전 클리어! 체력이 {healthReduction}만큼 감소하여 {player.Health}이(가) 되었습니다.");

            // 보상 계산
            int reward = CalculateReward(player);
            player.Gold += reward;
            Console.WriteLine($"던전 클리어 보상으로 {reward} G를 획득하였습니다.");
        }

        // 체력 감소량 계산
        private int CalculateHealthReduction(int playerDefense)
        {
            Random rand = new Random();
            int baseReduction = rand.Next(20, 36);
            int defenseDifference = playerDefense - RecommendedDefense;
            return baseReduction + defenseDifference;
        }

        // 보상 계산
        private int CalculateReward(Player player)
        {
            int baseReward = BaseReward;
            int attackBonusPercentage = new Random().Next(player.BaseAttackPower, player.BaseAttackPower * 2 + 1);
            int bonusReward = (baseReward * attackBonusPercentage) / 100;
            return baseReward + bonusReward;
        }
    }
    class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttackPower { get; set; }
        public int DefensePower { get; set; }
        public int Price { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsPurchased { get; set; }

        public Item(string name, string description, int attackPower = 0, int defensePower = 0, int price = 0, bool isEquipped = false, bool isPurchased = false)
        {
            Name = name;
            Description = description;
            AttackPower = attackPower;
            DefensePower = defensePower;
            Price = price;
            IsEquipped = isEquipped;
            IsPurchased = isPurchased;
        }

        public override string ToString()
        {
            string equippedMarker = IsEquipped ? "[E]" : " ";
            string purchasedMarker = IsPurchased ? "구매완료" : $"{Price} G";
            string stats = AttackPower > 0 ? $"공격력 +{AttackPower}" : $"방어력 +{DefensePower}";
            return $"{equippedMarker}{Name} | {stats} | {Description} | {purchasedMarker}";
        }
    }

    class Player
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public int Level { get; set; } = 1;
        public int BaseAttackPower { get; set; } = 10;
        public int BaseDefensePower { get; set; } = 5;
        public int Health { get; set; } = 100;
        public int Gold { get; set; } = 1500;
        public List<Item> Inventory { get; set; } = new List<Item>();

        public Player(string name, string job)
        {
            Name = name;
            Job = job;
        }

        public void EquipItem(int index)
        {
            if (index < 0 || index >= Inventory.Count)
            {
                Console.WriteLine("잘못된 선택입니다.");
                return;
            }

            Item item = Inventory[index];
            if (item.IsEquipped)
            {
                Console.WriteLine($"{item.Name}을(를) 해제했습니다.");
                item.IsEquipped = false;
            }
            else
            {
                Console.WriteLine($"{item.Name}을(를) 장착했습니다.");
                item.IsEquipped = true;
            }
        }

        public void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public void SellItem(int index)
        {
            if (index < 0 || index >= Inventory.Count)
            {
                Console.WriteLine("잘못된 선택입니다.");
                return;
            }

            Item item = Inventory[index];
            if (!item.IsPurchased)
            {
                Console.WriteLine("판매할 수 없는 아이템입니다.");
                return;
            }

            int sellPrice = (int)(item.Price * 0.85);
            Gold += sellPrice;

            if (item.IsEquipped)
            {
                EquipItem(index); // 장착 해제
            }

            Console.WriteLine($"{item.Name}을(를) {sellPrice} G에 판매했습니다.");
            Inventory.RemoveAt(index);
        }
        // 장착된 아이템의 총 공격력 보너스를 계산합니다.
        public int GetTotalEquippedAttackBonus()
        {
            int totalAttackBonus = 0;
            foreach (var item in Inventory)
            {
                if (item.IsEquipped)
                {
                    totalAttackBonus += item.AttackPower;
                }
            }
            return totalAttackBonus;
        }

        // 장착된 아이템의 총 방어력 보너스를 계산합니다.
        public int GetTotalEquippedDefenseBonus()
        {
            int totalDefenseBonus = 0;
            foreach (var item in Inventory)
            {
                if (item.IsEquipped)
                {
                    totalDefenseBonus += item.DefensePower;
                }
            }
            return totalDefenseBonus;
        }
    }

    class Shop
    {
        public List<Item> ItemsForSale { get; set; }

        public Shop()
        {
            ItemsForSale = new List<Item>
            {
                new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", defensePower: 5, price: 1000),
                new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", defensePower: 15, price: 3500),
                new Item("낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", attackPower: 2, price: 600),
                new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", attackPower: 5, price: 1500),
            };
        }

        public void DisplayShopItems(Player player)
        {
            Console.WriteLine("상점");
            Console.WriteLine("\n필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("\n[보유 골드] {0} G", player.Gold);
            Console.WriteLine("\n[아이템 목록]");

            foreach (var item in ItemsForSale)
            {
                Console.WriteLine(item);
            }

            foreach (var item in player.Inventory)
            {
                if (item.IsPurchased)
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("\n1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("\n원하시는 행동을 입력해주세요.");

            int choice = GetInput();
            if (choice == 1)
            {
                PurchaseItem(player);
            }
            else if (choice == 2)
            {
                SellItem(player);
            }
            else if (choice == 0)
            {
                Program.StartScene(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                DisplayShopItems(player);
            }
        }

        private void PurchaseItem(Player player)
        {
            Console.WriteLine("\n구매할 아이템의 번호를 입력하세요.");
            for (int i = 0; i < ItemsForSale.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ItemsForSale[i]}");
            }
            Console.WriteLine("0. 나가기");

            int choice = GetInput();
            if (choice == 0)
            {
                DisplayShopItems(player);
                return;
            }

            if (choice > 0 && choice <= ItemsForSale.Count)
            {
                Item selectedItem = ItemsForSale[choice - 1];
                if (selectedItem.IsPurchased)
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                    PurchaseItem(player);
                }
                else if (player.Gold >= selectedItem.Price)
                {
                    player.Gold -= selectedItem.Price;
                    selectedItem.IsPurchased = true;
                    player.AddItem(selectedItem);
                    Console.WriteLine($"{selectedItem.Name}을(를) 구매했습니다.");
                    DisplayShopItems(player);
                }
                else
                {
                    Console.WriteLine("골드가 부족합니다.");
                    PurchaseItem(player);
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                PurchaseItem(player);
            }
        }

        private void SellItem(Player player)
        {
            Console.WriteLine("\n판매할 아이템의 번호를 입력하세요.");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {player.Inventory[i]}");
            }
            Console.WriteLine("0. 나가기");

            int choice = GetInput();
            if (choice == 0)
            {
                DisplayShopItems(player);
                return;
            }

            if (choice > 0 && choice <= player.Inventory.Count)
            {
                player.SellItem(choice - 1);
                DisplayShopItems(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                SellItem(player);
            }
        }

        private int GetInput()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }
    }


    class Program
    {
        public static string PlayerJob(int playerJob)
        {
            return playerJob switch
            {
                1 => "전사",
                2 => "궁수",
                3 => "도적",
                4 => "마법사",
                _ => "알 수 없음",
            };
        }

        public static void StartScene(Player player)
        {
            Console.WriteLine("스파르타 마을에 오신 것을 환영합니다.");
            Console.WriteLine("\n1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("0. 나가기");

            int choice = GetInput();
            switch (choice)
            {
                case 1:
                    Status(player);
                    break;
                case 2:
                    InventoryManagement(player);
                    break;
                case 3:
                    new Shop().DisplayShopItems(player);
                    break;
                case 4:
                    EnterDungeon(player);
                    break;
                case 0:
                    Console.WriteLine("게임을 종료합니다.");
                    break;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    StartScene(player);
                    break;
            }
        }

        static void EnterDungeon(Player player)
        {
            List<Dungeon> dungeons = new List<Dungeon>
            {
                new Dungeon("쉬운 던전", 5, 1000),
                new Dungeon("일반 던전", 10, 1700),
                new Dungeon("어려운 던전", 15, 2500)
            };

            Console.WriteLine("\n입장할 던전을 선택하세요:");
            for (int i = 0; i < dungeons.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {dungeons[i].Name} (권장 방어력: {dungeons[i].RecommendedDefense})");
            }
            Console.WriteLine("0. 나가기");

            int choice = GetInput();
            if (choice == 0)
            {
                StartScene(player);
                return;
            }

            if (choice > 0 && choice <= dungeons.Count)
            {
                dungeons[choice - 1].Enter(player);
                StartScene(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                EnterDungeon(player);
            }
        }
        static void Status(Player player)
        {
            int totalAttackBonus = player.GetTotalEquippedAttackBonus();
            int totalDefenseBonus = player.GetTotalEquippedDefenseBonus();

            Console.WriteLine("\n상태보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine($"Lv. {player.Level}");
            Console.WriteLine($"{player.Name} ({player.Job})");
            Console.WriteLine($"공격력: {player.BaseAttackPower} (+{totalAttackBonus})");
            Console.WriteLine($"방어력: {player.BaseDefensePower} (+{totalDefenseBonus})");
            Console.WriteLine($"체력: {player.Health}");
            Console.WriteLine($"골드: {player.Gold} G");
            Console.WriteLine("\n0. 나가기");

            if (GetInput() == 0)
            {
                StartScene(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                Status(player);
            }
        }

        static void InventoryManagement(Player player)
        {
            Console.WriteLine("\n인벤토리 - 장착 관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("\n[아이템 목록]");

            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {player.Inventory[i]}");
            }

            Console.WriteLine("\n0. 나가기");

            int choice = GetInput();
            if (choice == 0)
            {
                StartScene(player);
            }
            else if (choice > 0 && choice <= player.Inventory.Count)
            {
                player.EquipItem(choice - 1);
                InventoryManagement(player);
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
                InventoryManagement(player);
            }
        }

        private static int GetInput()
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int result))
                {
                    return result;
                }
                Console.WriteLine("숫자를 입력해주세요.");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("사용하실 닉네임을 입력하세요.");
            string name = Console.ReadLine();

            Console.WriteLine("\n원하시는 직업을 선택하세요.");
            Console.WriteLine("1. 전사 2. 궁수 3. 도적 4. 마법사");
            int jobChoice = GetInput();
            string job = PlayerJob(jobChoice);

            Player player = new Player(name, job);

            Console.WriteLine();
            StartScene(player);
        }
    }
}
