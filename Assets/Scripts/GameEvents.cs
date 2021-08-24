using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class GameEvents {
    private static Subject<OrderData> _clientOrder = new Subject<OrderData>();
    public static IObservable<OrderData> GetClientOrder() => _clientOrder.AsObservable();
    public static void SetClientOrder(OrderData orderData) => _clientOrder.OnNext(orderData);

    private static Subject<uint> _complitedOrder = new Subject<uint>();
    public static IObservable<uint> GetComplitedOrder() => _complitedOrder.AsObservable();
    public static void SetComplitedOrder(uint clientID) => _complitedOrder.OnNext(clientID);

    private static Subject<uint> _showCookingPanel = new Subject<uint>();
    public static IObservable<uint> GetShowCookingPanel() => _showCookingPanel.AsObservable();
    public static void SetShowCookingPanel(uint cookID) => _showCookingPanel.OnNext(cookID);

    private static Subject<CookTaskData> _cookTask = new Subject<CookTaskData>();
    public static IObservable<CookTaskData> GetCookTask() => _cookTask.AsObservable();
    public static void SetCookTask(CookTaskData cookTaskData) => _cookTask.OnNext(cookTaskData);

    private static Subject<CookTaskData> _freeMachine = new Subject<CookTaskData>();
    public static IObservable<CookTaskData> GetFreeMachine() => _freeMachine.AsObservable();
    public static void SetFreeMachine(CookTaskData cookTaskData) => _freeMachine.OnNext(cookTaskData);

    private static Subject<MachineData> _machine = new Subject<MachineData>();
    public static IObservable<MachineData> GetMachine() => _machine.AsObservable();
    public static void SetMachine(MachineData machineData) => _machine.OnNext(machineData);

    private static Subject<ChosenMachineData> _chosenMachine = new Subject<ChosenMachineData>();
    public static IObservable<ChosenMachineData> GetChosenMachine() => _chosenMachine.AsObservable();
    public static void SetChosenMachine(ChosenMachineData chosenMachineData) => _chosenMachine.OnNext(chosenMachineData);

    private static Subject<CookingTimeData> _cookingTime = new Subject<CookingTimeData>();
    public static IObservable<CookingTimeData> GetCookingTime() => _cookingTime.AsObservable();
    public static void SetCookingTime(CookingTimeData cookingTimeData) => _cookingTime.OnNext(cookingTimeData);

    private static Subject<CookedMealData> _cookedMeal = new Subject<CookedMealData>();
    public static IObservable<CookedMealData> GetCookedMeal() => _cookedMeal.AsObservable();
    public static void SetCookedMeal(CookedMealData cookedMeal) => _cookedMeal.OnNext(cookedMeal);

    private static Subject<MealData> _removeMealFromPantry = new Subject<MealData>();
    public static IObservable<MealData> GetRemoveMealFromPantry() => _removeMealFromPantry.AsObservable();
    public static void SetRemoveMealFromPantry(MealData meal) => _removeMealFromPantry.OnNext(meal);

    private static Subject<QueueData> _clientsQueue = new Subject<QueueData>();
    public static IObservable<QueueData> GetClientsQueue() => _clientsQueue.AsObservable();
    public static void SetClientsQueue(QueueData queueData) => _clientsQueue.OnNext(queueData);

}