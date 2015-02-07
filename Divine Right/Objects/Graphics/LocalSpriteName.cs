﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Graphics
{
    public enum LocalSpriteName
    {
        //Tiles
        WOOD_TILE_1,
        GRASS_TILE,
        ROAD_TILE,
        SOIL_TILE,
        WET_SOIL_TILE,
        CAVE_TILE,
        MARBLE_TILE,
        PAVEMENT_TILE_1,
        PAVEMENT_TILE_2,
        WOOD_TILE_2,
        WOOD_TILE_3,
        WOOD_TILE_4,
        WOOD_TILE_5,
        DUNGEON_TILE,
        LAVA_TILE,
        WATER_TILE,
        NONE,

        //Town Indoor Items
        BLUE_BED,
        WOOD_CHAIR_LEFT,
        WOOD_CHAIR_RIGHT,
        WOOD_CHAIR_BOTTOM,
        WOOD_CHAIR_TOP,
        EMPTY_SHELFING,
        BOOKCASE_FULL,
        BOOKCASE_HALF_EMPTY,
        GLASSWARE_CABINET,
        SMALL_RUG,
        CHEST_OF_DRAWERS,
        CHEST_OF_DRAWERS_1,
        CHEST_OF_DRAWERS_2,
        CHEST_OF_DRAWERS_3,
        CHEST_OF_DRAWERS_4,
        CABINET_1,
        WARDROBE_1,
        WARDROBE_2,
        WARDROBE_3,
        WARDROBE_4,
        STOVE_UNLIT,
        STOVE_LIT,
        PUMP,
        DISHES,
        TABLE_1,
        DOOR_CLOSED,
        DOOR_OPEN,
        FOUNTAIN_SMALL,
        STATUE_AXEMAN,
        WHITE_BED,
        PLANT_POT_1,
        PLANT_POT_2,
        TABLE_2,
        BEARSKIN_RUG,
        BOOKCASE_LARGE,
        BLUE_WALL,
        POTS,
        RED_BED_DOUBLE,
        RED_BED,
        TABLE_3,
        TABLE_4,
        LAMP_1,
        LAMP_2,
        PLANT_POT_3,
        NICE_CHAIR_RIGHT,
        NICE_CHAIR_LEFT,
        NICE_CHAIR_TOP,
        NICE_CHAIR_BOTTOM,
        BLUE_POT_LARGE,
        BARSTOOL,
        BLUE_POT_SMALL,
        DARK_RED_WALL,
        LIGHT_RED_WALL,
        BARREL,
        BARRELS,
        CHEST_CLOSED,
        CHEST_OPEN,
        PLAYERCHAR_MALE,

        BOOKCASE_1,
        BOOKCASE_2,
        BOOKCASE_3,
        BOOKCASE_4,

        DESK_1,
        DESK_2,
        DESK_3,

        PLANT_CABINET_1,
        PLANT_CABINET_2,
        PLANT_CABINET_3,

        WINDOW_1,
        WINDOW_2,
        WINDOW_3,

        DOOR_1_OPEN,
        DOOR_1_CLOSED,
        DOOR_2_OPEN,
        DOOR_2_CLOSED,

        POT_PLANT_1,
        POT_PLANT_2,
        POT_PLANT_3,
        POT_PLANT_4,
        POT_PLANT_5,

        BATH,

        CARPET_1,
        CARPET_2,
        CARPET_3,

        RED_BED_TOP,
        RED_BED_BOTTOM,

        BUCKET,

        BAR,

        LARGE_TABLE_TL,
        LARGE_TABLE_TR,
        LARGE_TABLE_BL,
        LARGE_TABLE_BR,

        FOOD_CABINET_1,
        FOOD_CABINET_2,

        LARGE_TABLE_TEA_TL,
        LARGE_TABLE_TEA_TR,
        LARGE_TABLE_TEA_BL,
        LARGE_TABLE_TEA_BR,

        //Resources
        WOOD_LOGS,
        WOOD_LOGS_THREE,
        FLOUR_SACK,
        FLOUR_SACK_THREE,
        GRAIN_OPEN,
        GRAIN_OPEN_THREE,
        GREEN_OPEN,
        GREEN_OPEN_THREE,
        STEAK,
        STEAK_THREE,
        DRUMSTICKS,
        DRUMSTICKS_THREE,
        CHICKEN,
        CHICKEN_THREE,
        MEAT,
        MEAT_THREE,
        ANVIL,
        ANVIL_2,
        SMITH_TOOLS_1,
        SMITH_TOOLS_2,
        SMITH_TOOLS_3,
        EMPTY_STALL,
        VEG_1,
        VEG_STALL_1,
        VEG_2,
        VEG_STALL_2,
        VEG_3,
        VEG_STALL_3,
        VEG_4,
        VEG_STALL_4,
        VEG_5,
        VEG_STALL_5,
        VEG_6,
        VEG_STALL_6,
        VEG_7,
        VEG_STALL_7,
        FISH_1,
        FISH_2,
        FISH_3,
        FISH_4,
        GOLD_BARS_1,
        GOLD_BARS_2,
        BREAD_THREE,
        BREAD_TWO,
        FLOUR_MILL,
        BOXES,
        TREE_STUMPS,
        PICKAXE,
        HATCHET_SHOVEL,
        MILK_JUGS,
        OIL_JARS,
        GOLD_ORE,
        IRON_ORE,
        COPPER_ORE,
        MINING_TOOLS,
        FURNACE,
        IRON_BARS_1,
        IRON_BARS_2,

        ANGEL_STATUE_TOP,
        ANGEL_STATUE_BOTTOM,

        FLOWER_1,
        FLOWER_2,
        FLOWER_3,
        FLOWER_4,
        FLOWER_5,
        FLOWER_6,
        FLOWER_7,
        FLOWER_8,
        FLOWER_9,
        FLOWER_10,
        FLOWER_11,
        FLOWER_12,
        FLOWER_13,
        FLOWER_14,
        FLOWER_15,
        FLOWER_16,
        FLOWER_17,
        BENCH_LEFT,
        BENCH_RIGHT,
        BENCH_DOWN,
        FLOWER_18,
        FLOWER_19,
        FLOWER_20,
        FLOWER_21,
        FLOWER_22,
        FLOWER_23,
        FOUNTAIN_1,
        FLOWER_24,
        BENCH_2_DOWN,
        FLOWER_25,
        FLOWER_26,
        COLUMN,
        FOUNTAIN_2,
        POTS_1,
        FLOWER_27,
        GIRL_STATUE,
        WELL_1,
        FLOWER_28,

        TOMATO_PLANT,
        POTATO_PLANT,
        CARROT_PLANT,
        ARTICHOKE_PLANT,
        PEPPER_PLANT,
        COURGETTE_PLANT,
        CORN_PLANT,
        TOMATO,
        POTATO,
        CARROT,
        ARTICHOKE,
        PEPPERS,
        COURGETTE,
        CORN,

        //General store stuff
        PEEL_1,
        PEEL_2,
        FURNACE_LARGE_TOP,
        FURNACE_LARGE_BOTTOM,
        ROLLING_PIN,
        BREAD,
        CHEESE,
        BASKET_1,
        BASKET_2,
        MILK,
        SUGAR,
        FLOUR,
        GRAIN_OPEN_2,
        GRAIN_OPEN_SMALL,

        FOOD_SHELF_1,
        FOOD_SHELF_2,
        FOOD_SHELF_3,
        FOOD_SHELF_4,
        FOOD_SHELF_5,
        FOOD_SHELF_6,
        FOOD_SHELF_7,
        FOOD_SHELF_8,
        FOOD_SHELF_9,
        FOOD_SHELF_10,

        BREAD_BASKET,
        CHEESE_BASKET,
        BREAD_BASKET_2,
        SUGAR_BASKET,

        FOOD_PRODUCE,


        //Forests and Mines and stuff
        TREE_1,
        TREE_2,
        TREE_3,
        DEAD_TREE,
        TREE_4,
        TREE_5,
        TREE_6,
        TREE_7,

        //Dungeons
        STONE_1,
        STONE_2,
        STONE_3,
        STONE_4,
        STONE_5,
        STONE_6,
        STONE_7,
        STONE_8,
        STONE_9,
        STONE_10,

        BIG_STONE_1,
        BIG_STONE_2,
        BIG_STONE_3,
        BIG_STONE_4,

        BONES_1,
        BONES_2,
        BONES_3,

        BIG_STONE_5,
        BIG_STONE_6,
        BIG_STONE_7,
        BIG_STONE_8,
        BIG_STONE_9,
        BIG_STONE_10,

        STONE_11,
        STONE_12,
        STONE_13,
        STONE_14,
        STONE_15,

        BIG_STONES_TL,
        BIG_STONES_TR,
        BIG_STONES_BL,
        BIG_STONES_BR,

        BIG_STONE_11,
        STONE_16,
        STONE_17,
        STONE_18,

        BONES_4,
        BONES_5,
        BONES_6,
        BONES_7,

        DRAGON_STATUE_1,
        DRAGON_STATUE_2,

        TREASURE_1,
        TREASURE_2,

        CANDLE_1,
        CANDLE_2,

        ARMOUR_RACK_1,
        ARMOUR_RACK_2,
        WEAPON_RACK_1,
        WEAPON_RACK_2,

        SPIKES,

        //ENEMIES
        ENEMY_SKELETON,
        ENEMY_ORC_CIV,
        ENEMY_ORC_LIGHT,
        ENEMY_ORC_HEAVY,

        DIRE_RAT,
        DIRE_BEAR,

        //ENEMY THOUGHTS
        ENEMY_THOUGHT_WALK,
        ENEMY_THOUGHT_WAIT,
        ENEMY_THOUGHT_ATTACK,
        ENEMY_THOUGH_CONFUSED,
        WATER_TROUGH,
        FOOD_TROUGH,
        HAY_BALE,
        CARPENTRY_HAMMER_1,
        CARPENTRY_HAMMER_2,
        HATCHET,
        SAW,
        CARPENTRY_HAMMER_3,
        BOW_SAW_1,
        BOW_SAW_2,
        CARPENTRY_WORKBENCH,
        LOG_PILE,
        CARPENTRY_WORKBENCH_2,
        CARPENTRY_WORKBENCH_3,
        CARPENTRY_WORKBENCH_4,
        CARPENTRY_WORKBENCH_5,
        WOOD_LOGS_2,
        CRATE_EMPTY,
        CRATE_1,
        CRATE_2,
        SAWHORSE_1,
        SAWHORSE_2,
        PLANKS_1,
        PLANKS_2,
        WEAPON_RACK_3,
        MANICLES_1,
        MANICLES_2,
        MANICLES_3,
        MANICLES_4,
        STOCKS_1,
        STOCKS_2,
        HUMANGUARD1,
        HUMANGUARD2,
        HUMANGUARD3,
        HUMANGUARD4,
        HUMANGUARD5,
        HUMANGUARD6,
        HUMANGUARD7,
        HUMANMERCHANT1,
        HUMANMERCHANT2,
        HUMANMERCHANT3,
        HUMANMERCHANT4,
        HUMANMERCHANT5,
        HUMANPOOR1,
        HUMANPOOR2,
        HUMANPOOR3,
        HUMANPOOR5,
        HUMANPOOR4,
        HUMANPOOR6,
        HUMANRICH6,
        HUMANRICH5,
        HUMANRICH4,
        HUMANRICH1,
        HUMANRICH2,
        HUMANRICH3,
        HUMANMERCHANT_HAIR,
        HUMANMERCHANT_HEAD,
        HUMANMERCHANT_BODY,
        HUMANGUARD_HAIR,
        HUMANGUARD_BODY,
        HUMANPEASANTGIRL_HAIR,
        HUMANPEASANTGIRL_FACE,
        HUMANPEASANTGIRL_DRESS1,
        HUMANPEASANTGIRL_DRESS2,
        HUMANPEASANTMALE_HAIR,
        HUMANPEASANTMALE_FACE,
        HUMANPEASANTMALE_TOP,
        HUMANPEASANTMALE_PANTS,
        RICHMALE_HAIR,
        RICHMALE_FACE,
        RICHMALE_CLOTHES,
        RICHFEMALE_HAIR,
        RICHFEMALE_FACE,
        RICHFEMALE_CLOTHES,
        PRIEST_BODY,
        PRIEST_CLOTHES,
        GREY_GEM,
        BROWN_GEM,
        GREEN_GEM,
        BLUE_GEM,
        RED_GEM,
        RED_GEM_RING,
        BLUE_GEM_RING,
        GREEN_GEM_RING,
        BROWN_GEM_RING,
        GREY_GEM_RING,
        GREY_GEM_PENDANT,
        BROWN_GEM_PENDANT,
        GREEN_GEM_PENDANT,
        BLUE_GEM_PENDANT,
        RED_GEM_PENDANT,
        CLOTH_ARMOUR,
        LEATHER_ARMOUR,
        CHAIN_ARMOUR,
        LIGHT_BREASTPLATE,
        HEAVY_BREASTPLATE,
        HEAVY_ARMOUR,
        ELITE_ARMOUR,
        LEGENDARY_ARMOUR,
        COINS,
        HELM_1,
        HELM_2,
        HELM_3,
        HELM_4,
        HELM_5,
        HELM_6,
        HELM_7,
        HELM_8,
        SHIELD_1,
        SHIELD_2,
        SHIELD_3,
        SHIELD_4,
        SHIELD_5,
        SHIELD_6,
        SHIELD_7,
        SHIELD_8,
        SWORD_1,
        SWORD_2,
        SWORD_3,
        SWORD_4,
        SWORD_5,
        SWORD_6,
        SWORD_7,
        SWORD_8,
        AXE_1,
        AXE_2,
        AXE_3,
        AXE_4,
        AXE_5,
        AXE_6,
        AXE_7,
        AXE_8,
        PLATE_LEGGINGS,
        CHAIN_LEGGINGS,
        LEATHER_LEGGINGS,
        PADDED_LEGGINGS,
        PLAYERCHAR_FEMALE,
        PALLISADE_LR,
        PALLISADE_TB,
        BEDROLL,
        BANDIT_EASY,
        BANDIT_HARD,
        BANDIT_MEDIUM,
        CAMPFIRE,
        PALLISADE_BT,
        PALLISADE_RL,
        MOOSE,
        RAT,
        CAMEL_1,
        CAMEL_2,
        COUGAR,
        ELEPHANT,
        POLAR_BEAR,
        WOLF_1,
        WOLF_2,
        GIANT_SNAKE,
        GIANT_LIZARD_1,
        GIANT_LIZARD_2,
        CHICKEN_1,
        COW,
        LLAMA,
        PIG,
        SHEEP,
        WALRUS,
        GIANT_TURTLE,
        RABBIT,
        PANTHER,
        BEAR,
        HIDE,
        JUNGLE_TILE,
        JUNGLE_TREE_1,
        JUNGLE_TREE_2,
        JUNGLE_TREE_3,
        CACTUS_1,
        CACTUS_2,
        SAND,
        SNOW,
        DEAD_TREE_2,
        DEAD_TREE_3,
        SNOW_BUSH,
        SNOW_TREE_3,
        SNOW_TREE_2,
        SNOW_TREE_1,
        SNOW_2,
        SWAMP,
        GARRIGUE,
        SHRUB_1,
        SHRUB_2,
        TREE_8,
        TREE_9,
        UNHOLY_STATUE_TL,
        UNHOLY_STATUE_TR,
        UNHOLY_STATUE_BL,
        UNHOLY_STATUE_BR,
        MINE_SHAFT_TL,
        MINE_SHAFT_TC,
        MINE_SHAFT_TR,
        MINE_SHAFT_CL,
        MINE_SHAFT_CC,
        MINE_SHAFT_CR,
        MINE_SHAFT_BL,
        MINE_SHAFT_BC,
        MINE_SHAFT_BR,
        ENEMY_ORC_MEDIUM,
        ENEMY_ORC_PRIEST,
        HUMAN_FIGHTER_LIGHT,
        HUMAN_FIGHTER_MEDIUM,
        HUMAN_FIGHTER_HEAVY,
        ENEMY_THOUGHT_PRONE,
        FISHING_ROD,
        FISH_STALL_1,
        FISH_STALL_2,
        FISH_STALL_3,
        FISH_STALL_4,
        FISH_STALL_5,
        FISHING_BOAT_RIGHT,
        FISHING_BOAT_LEFT,
        TABLE_FOR_1,
        WALL_WOOD_D,
        WALL_WOOD_R,
        WALL_WOOD_U,
        WALL_WOOD_L,
        HAY_BALE_3,
        HAY_BALE_2,
        WOOD_LOGS_3,
        TREE_STUMP_HATCHET,
        TREE_STUMPS_2,
        TREE_STUMPS_3,
        TREE_STUMPS_4,
        TANNING_RACK,
        MEAT_TABLE,
        MEAT_TABLE2,
        MEAT_TABLE3,
        KNIFE_SET,
        BEAR_TRAP,
        HORSE1,
        HORSE3,
        HORSE2,
        FENCE_LEFT,
        FENCE_TOP,
        FENCE_RIGHT,
        FENCE_BOTTOM,
        DUNGEON_WALL,
        STAIRS_DOWN,
        STAIRS_UP,
        SUMMONING_CIRCLE,
        ALTAR,
        SMASHED_POT,
        HATCH,
        PILLAR,
        PILLAR_2,
        PILLAR_3,
        SKELETON_EASY,
        SKELETON_MEDIUM,
        SKELETON_PRIEST,
        LICH,
        MUMMY_1,
        MUMMY_2,
        WRAITH,
        ZOMBIE_1,
        ZOMBIE_2,
        ZOMBIE_3,
        ZOMBIE_4,
        ZOMBIE_5,
        ZOMBIE_6,
        SUMMONING_CIRCLE_OFF,
        TREASURE_CLOSED,
        TREASURE_EMPTY,
        ALTAR_CURSED,
        ALTAR_USED,
        BLOOD_1,
        BLOOD_2,
        BLOOD_3,
        POTION_BRAWN,
        POTION_CHARISMA,
        POTION_AGIL,
        POTION_PERC,
        POTION_INTEL,
        POTION_UNKNOWN,
        POTION_FEED,
        POTION_DEFEND,
        POTION_ATTACK,
        POTION_HEAL,
        POTION_ARMOUR,
        POTION_BLIND,
        POTION_BLEED,
        POTION_PARALYSE,
        POTION_FIRE,
        POTION_ICE,
        POTION_POISON,
        ALCHEMY_TABLE2,
        ALCHEMY_TABLE1,
        ALCHEMY_TABLE3,
        ALCHEMY_CABINET2,
        ALCHEMY_CABINET1,
        ALCHEMY_TABLE4,
        JAR_STORE1,
        JAR_STORE2,
        UNDERGROUND_PLANT7,
        UNDERGROUND_PLANT6,
        UNDERGROUND_PLANT5,
        UNDERGROUND_PLANT4,
        UNDERGROUND_PLANT3,
        UNDERGROUND_PLANT2,
        UNDERGROUND_PLANT1,
        CAGE,
        PRISON_WALL,
        HEADSTONE_6,
        HEADSTONE_5,
        HEADSTONE_4,
        HEADSTONE_3,
        HEADSTONE_2,
        HEADSTONE_1,
        RACK_LR_R,
        RACK_LR_L,
        RACK_TB_T,
        RACK_TB_B,
        GUILLOTINE_T,
        GUILLOTINE_B,
        BOW_1,
        BOW_2,
        BOW_3,
        CROSSBOW_1,
        CROSSBOW_2,
        ARROWS,
        BOLTS,
        ROGUE_CLASS,
        WARRIOR_CLASS,
        BRUTE_CLASS,
        RANGED_CLASS,
        SCROLL,
        SA_GREEN,
        SA_ORANGE,
        SA_DARKRED,
        SA_YELLOW,
        SA_RED,
    }
}
